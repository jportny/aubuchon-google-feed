using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Mozu.Api;
using Mozu.Api.Contracts.Customer;
using Mozu.Api.Contracts.Event;
using Mozu.Api.Resources.Commerce.Customer;
using Mozu.Api.ToolKit.Config;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Mozu.AubuchonDataAdapter.Domain.Handlers;

namespace Mozu.AubuchonDataAdapter.Domain.Events
{
    public class AccountEventProcessor : IEventProcessor
    {
        private readonly IAubuchonAccountHandler _aubuchonAccountHandler;
        private readonly IAccountPhoneHandler _accountPhoneHandler;
        private readonly IAppSetting _appSetting;
        private readonly IEventHandler _eventHandler;
        private bool _isLive;
        private readonly ILog _logger = LogManager.GetLogger(typeof(AccountEventProcessor));
        public AccountEventProcessor(IAubuchonAccountHandler aubuchonAccountHandler, IEventHandler eventHandler, IAccountPhoneHandler accountPhoneHandler, IAppSetting appSetting)
        {
            _aubuchonAccountHandler = aubuchonAccountHandler;
            _accountPhoneHandler = accountPhoneHandler;
            _appSetting = appSetting;
            _eventHandler = eventHandler;
        }
        public async Task ProcessEvent(IApiContext apiContext, Event evt, StatusCode statusCode = StatusCode.Active)
        {
            try
            {
                _isLive = _appSetting.Settings.ContainsKey("IsLive") && Convert.ToBoolean(_appSetting.Settings["IsLive"]);
                var accountId = Convert.ToInt32(evt.EntityId);
                var aubEvent = evt.ToAubEvent();

                var customerResource = new CustomerAccountResource(apiContext);

                var account = await customerResource.GetAccountAsync(accountId);
                var isPreLiveCustomer = account.IsPreLiveCustomer();
                var isRewardsAssigned = account.IsRewardsAssigned();

                if (_isLive && account.IsAnonymous && isPreLiveCustomer) return;

                if (!aubEvent.Topic.Equals("customeraccount.updated") && !account.Attributes.Any(a => a.FullyQualifiedName.Equals(AttributeConstants.PostLiveEdgeAccount))) //Add to default segment only for customer create
                {
                    await _aubuchonAccountHandler.AddToSegment(apiContext, accountId, SegmentConstants.HomeTeam);
                }

                //Check if Aubuchon Id is assigned
                //
                if (account.Contacts.Any() && !account.IsAnonymous)
                {
                    var aubuchonId = await _aubuchonAccountHandler.AddUpdateAccountAsync(account) ?? account.ExternalId;
                    if (aubuchonId != null)
                    {
                        account.ExternalId = aubuchonId;
                        account = account.FormatAccount();
                        await customerResource.UpdateAccountAsync(account, accountId, "Id,ExternalId");

                    }
                }

                //if (!_isLive)
                //{
                //AddEvent PhoneNumbers to Entities
                await AddUpdateAccountPhone(apiContext.TenantId, account, statusCode);
                //}

                //AddEvent reward for existing Customer
                if (statusCode == StatusCode.Active && !account.IsAnonymous && !isRewardsAssigned && isPreLiveCustomer)
                {
                    _logger.Info(String.Format("Calling Rewards: {0} / anonymous :{1} / isrewardsassigned: {2} / isprelive :{3} / account : {4}", statusCode, !account.IsAnonymous, !isRewardsAssigned, isPreLiveCustomer, accountId));
                    await AddRewardForExisitingCustomerAsync(apiContext, accountId);
                }

                if (!account.IsEdgeImportCustomer() && !account.IsAnonymous)
                {

                    //Export Customer data to Edge
                    var customerHandler = new EdgeCustomerExportHandler(_appSetting);

                    await customerHandler.ExportCustomerAsync(apiContext, account, statusCode);
                }
                else
                {
                    await account.ResetEdgeImportFlag(apiContext);
                }

            

                await MarkAsProcessed(apiContext, aubEvent);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private async Task MarkAsProcessed(IApiContext apiContext, AubEvent aubEvent)
        {
            //Set Event as Processed
            aubEvent.Status = EventStatus.Processed.ToString();
            aubEvent.ProcessedDateTime = DateTime.UtcNow;
            await _eventHandler.UpdateEvent(apiContext.TenantId, aubEvent);
        }

        private async Task AddUpdateAccountPhone(int tenantId, CustomerAccount account, StatusCode statusCode)
        {
            foreach (var phone in account.Contacts.SelectMany(contact => BuildAccountPhones(account, contact.PhoneNumbers.Home, contact.PhoneNumbers.Mobile, contact.PhoneNumbers.Work)))
            {
                await
                    _accountPhoneHandler.UpsertAccountPhoneAsync(tenantId, phone);
            }
        }

        private static IEnumerable<AccountPhone> BuildAccountPhones(CustomerAccount account, params string[] phoneNumbers)
        {
            return (from phoneNumber in phoneNumbers
                    where !String.IsNullOrEmpty(phoneNumber)
                    select new AccountPhone
                    {
                        AccountId = account.Id,
                        Id = String.Format("{0}-{1}", account.Id, phoneNumber),
                        Phone = phoneNumber
                    }).ToList();
        }

        private async Task AddRewardForExisitingCustomerAsync(IApiContext apiContext, int accountId)
        {
            //this assumes that since this is an exisitng account, the import will have the aubuchon in mozu already
            var accountHandler = new Mozu.Api.Resources.Commerce.Customer.Accounts.CustomerAttributeResource(apiContext);
            var existingPreLive = await accountHandler.GetAccountAttributeAsync(accountId, AttributeConstants.ExistingAccountPreLive);
            if (existingPreLive != null && existingPreLive.Values.Count > 0 && (bool)existingPreLive.Values[0])
                await _aubuchonAccountHandler.AddRewardForExisitingCustomer(apiContext, accountId);
            else
            {
                _logger.Info(String.Format("Rewards not assigned for account {0}", accountId));
            }
        }
    }
}
