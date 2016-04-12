using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Mozu.Api;
using Mozu.Api.Contracts.Customer;
using Mozu.Api.Resources.Commerce.Customer;
using Mozu.Api.ToolKit.Config;
using Mozu.Api.ToolKit.Readers;
using Mozu.AubuchonDataAdapter.Domain.Loyalty;



namespace Mozu.AubuchonDataAdapter.Domain.Handlers
{
    public interface IAubuchonAccountHandler
    {
        Task AddToSegment(IApiContext apiContext, int accountId, string segmentCode);
        Task<string> AddUpdateAccountAsync(CustomerAccount account);
        Task<CustomerAccount> GetCustomer(string accountId);
        Task<double> GetCustomerRewardPoints(IApiContext context, int mozuCustomerId);
        Task AddCustomerRewardPoints(IApiContext context, int mozuAccountId, RewardsToBeCreated[] rewardsToBeCreated);
        Task AddRewardForExisitingCustomer(IApiContext context, int mozuAccountId);
        Task<Dictionary<string, int>> RemoveCustomerRewardPoints(IApiContext context, int mozuAccountId, RewardsToBeReserved[] rewardsToBeReserved);
    }

    public class AubuchonAccountHandler : HandlerBase, IAubuchonAccountHandler
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(AubuchonAccountHandler));

        public AubuchonAccountHandler(IAppSetting appSetting)
            : base(appSetting)
        {

        }

        public async Task AddToSegment(IApiContext apiContext, int accountId, string segmentCode)
        {
            var customerResource = new CustomerAccountResource(apiContext);
            var account = await customerResource.GetAccountAsync(accountId, "segments").ConfigureAwait(false);
            if (account.Segments.Any(s => s.Code == segmentCode)) return;

            var segments = new List<CustomerSegment>();
            var customerSegmentResouce = new CustomerSegmentResource(apiContext);

            var reader = new CustomerSegmentReader { Context = apiContext, PageSize = 200, ResponseFields = "Items(Id, Code)" };
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                segments.AddRange(reader.Items);
            }
            var segment = segments.SingleOrDefault(s => s.Code == segmentCode);
            if (segment != null)
            {
                await customerSegmentResouce.AddSegmentAccountsAsync(new List<int> { accountId }, segment.Id).ConfigureAwait(false);
            }
        }

        public async Task<string> AddUpdateAccountAsync(CustomerAccount account)
        {
            try
            {
                if (account.IsAnonymous) return account.ExternalId;
                _logger.Info(String.Format("Calling AddUpdateAccount for {0}, with external Id {1}", account.Id, account.ExternalId));
                var request = new CustomerAddUpdateRequest
                {
                    _customerData = new CustomerRequest
                    {
                        MozuCustomerid = account.Id.ToString(CultureInfo.InvariantCulture),
                        CustomerId = account.ExternalId,
                        RequestorID = "Mozu",
                        RequestorKey = "BCBEA201-7590-E411-A7C4-E0DB5501CCAC",
                        businessname = account.CompanyOrOrganization,
                        firstname = account.FirstName,
                        middleInit = String.Empty,
                        lastname = account.LastName
                    }
                };

                _logger.Info(request.ToXml());

                var address = account.Contacts.FirstOrDefault(c => c.Types.Any(t => t.Name == "Billing" && t.IsPrimary)) ??
                              account.Contacts.FirstOrDefault(c => c.Types.Any(t => t.Name == "Billing")) ??
                              account.Contacts.FirstOrDefault();

                //TODO: Check if the first address should be sent if there is no default billing

                if (address != null)
                {
                    var attribBirthDay =
                        account.Attributes.FirstOrDefault(a => a.FullyQualifiedName.Equals("tenant~birth-day") && a.Values.Any());
                    var attribBirthMonth = account.Attributes.FirstOrDefault(a => a.FullyQualifiedName.Equals("tenant~birth-month") && a.Values.Any());
                    request._customerData.addr1 = address.Address.Address1;
                    request._customerData.addr2 = address.Address.Address2;
                    request._customerData.city = address.Address.CityOrTown;
                    request._customerData.state = address.Address.StateOrProvince;
                    request._customerData.zip = address.Address.PostalOrZipCode;
                    request._customerData.CountryCode = address.Address.CountryCode;
                    request._customerData.phonenumber = address.PhoneNumbers.Home;
                    request._customerData.emailaddr = address.Email;
                    request._customerData.birthday = attribBirthDay != null ? attribBirthDay.Values.First().ToString() : String.Empty;
                    request._customerData.birthmth = attribBirthMonth != null ? attribBirthMonth.Values.First().ToString() : String.Empty;
                    request._customerData.maillist = account.AcceptsMarketing ? "1" : "0";
                    request._customerData.taxexnumber = account.TaxId;
                    request._customerData.ckprivilege = "No";
                    request._customerData.type = String.Empty;

                    if (String.IsNullOrEmpty(address.PhoneNumbers.Home))
                    {
                        request._customerData.phonenumber = !String.IsNullOrEmpty(address.PhoneNumbers.Mobile)
                            ? address.PhoneNumbers.Mobile
                            : address.PhoneNumbers.Work;
                    }
                }


                using (var service = new ServiceClient())
                {

                    var response = await service.CustomerAddUpdateAsync(request);
                    return response.CustomerAddUpdateResult.customerid;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(String.Format("AubuchonId :{0}, Mozu Customer Id :{1}, Err: {2}", account.ExternalId,account.Id,ex.Message));
            }
            return null;
        }


        public async Task<CustomerAccount> GetCustomer(string accountId)
        {
            var request = new GetCustomerFullRequest
            {
                _customer = new customerRewardsPointsRequest
                {
                    CustomerId = accountId,
                    RequestorID = "Mozu",
                    RequestorKey = "BCBEA201-7590-E411-A7C4-E0DB5501CCAC"
                }
            };
            using (var service = new ServiceClient())
            {
                var response = await service.GetCustomerFullAsync(request);
                var account = new CustomerAccount
                {
                    ExternalId = response.GetCustomerFullResult.CustomerId,
                    EmailAddress = response.GetCustomerFullResult.emailaddr,
                    FirstName = response.GetCustomerFullResult.firstname,
                    LastName = response.GetCustomerFullResult.lastname
                };
                return account;
            }
        }
        /// <summary>
        /// Gets Aubuchon customer reward points from Aubuchon web service
        /// </summary>
        /// <param name="context"> Mozu Api context</param>
        /// <param name="mozuCustomerId">Mozu CustomerId/ Mozu ExternalId </param>
        /// <returns>points</returns>
        public async Task<double> GetCustomerRewardPoints(IApiContext context, int mozuCustomerId)
        {
            var customer = await (new CustomerAccountResource(context)).GetAccountAsync(mozuCustomerId);

            if (customer == null)
                throw new Exception("Not Found");

            //is it possible for customer to not have aubuchon id? 
            if (String.IsNullOrWhiteSpace(customer.ExternalId))
                throw new Exception("No external Id");

            using (var service = new ServiceClient())
            {
                var request = new GetCustomerRewardsPointsRequest
                {
                    customerRewardsPointsRequest = new customerRewardsPointsRequest
                    {
                        CustomerId = customer.ExternalId,
                        RequestorID = "Mozu",
                        RequestorKey = "BCBEA201-7590-E411-A7C4-E0DB5501CCAC"
                    }
                };
                var response = service.GetCustomerRewardsPoints(request);
                if (!response.GetCustomerRewardsPointsResult.HasErrors)
                    return response.GetCustomerRewardsPointsResult.PointTotal;
                _logger.Error(String.Format("{0} : {1}", response.GetCustomerRewardsPointsResult.ErrorMessage,
                    customer.ExternalId));
                return 0;
            }
        }
        public async Task AddRewardForExisitingCustomer(IApiContext context, int mozuAccountId)
        {
            await AddCustomerRewardPoints(context, mozuAccountId,
                new[]
                { 
                    new RewardsToBeCreated{
                    OrderNo = "0",
                    RewardsAmt = 5,
                    RewardsType = "R",
                    RewardsLocation = "991"
                    }
                });
        }

        public async Task AddCustomerRewardPoints(IApiContext context, int mozuAccountId, RewardsToBeCreated[] rewardsToBeCreated)
        {
            //double rewardAmount, string orderNumber, string rewardLocation) {

            var customer = await (new CustomerAccountResource(context).GetAccountAsync(mozuAccountId));
            if (String.IsNullOrWhiteSpace(customer.ExternalId))
                return;
            var request = new RequestCreateRewardRequest
            {
                _rewardCreateList = new RequestCreateRewardList
                {
                    CustomerId = customer.ExternalId,
                    ListOfRewards = rewardsToBeCreated,
                    RequestorID = "Mozu",
                    RequestorKey = "BCBEA201-7590-E411-A7C4-E0DB5501CCAC"
                }
            };

            using (var service = new ServiceClient())
            {
                var response = service.RequestCreateRewardAsync(request).Result;
                var reward = response.RequestCreateRewardResult.ListOfReservedRewards.FirstOrDefault();
                if ((reward != null && reward.HasErrors) || response.RequestCreateRewardResult.HasErrors)
                {
                    
                    _logger.Error(String.Format("{0} : {1}", reward != null ? reward.ErrorMessage : response.RequestCreateRewardResult.ErrorMessage, customer.ExternalId));
                }
                else
                {
                    _logger.Info(String.Format("Setting RewardsAssigned flag to true for account {0}, External Id {1}", customer.Id, customer.ExternalId));
                    await customer.SetRewardsAssignedFlag(context);
                }
            }

        }

        public async Task<Dictionary<string, int>> RemoveCustomerRewardPoints(IApiContext context, int mozuAccountId, RewardsToBeReserved[] rewardsToBeReserved)
        {
            var customer = await (new CustomerAccountResource(context).GetAccountAsync(mozuAccountId));
            if (String.IsNullOrWhiteSpace(customer.ExternalId))
                return new Dictionary<string, int>();
            var request = new RequestReserveRewardRequest
            {
                _reseveRewardsList = new RequestReserveRewardList
                {
                    CustomerId = customer.ExternalId,
                    ListOfRewards = rewardsToBeReserved,
                    RequestorID = "Mozu",
                    RequestorKey = "BCBEA201-7590-E411-A7C4-E0DB5501CCAC",
                }
            };

            using (var service = new ServiceClient())
            {
                var response = service.RequestReserveReward(request);
                var reward = response.RequestReserveRewardResult.ListOfReservedRewards.FirstOrDefault();
                if (reward != null)
                {
                    if (!reward.HasErrors)
                    {
                        return response.RequestReserveRewardResult.ListOfReservedRewards.
                            Select(r => new {r.RewardsNo, r.authID}).
                            ToDictionary(t => t.RewardsNo, t => t.authID);
                    }
                    _logger.Error(String.Format("{0} : {1}", reward.ErrorMessage, customer.ExternalId));
                   
                }
            }
            return null;
        }

        //static Task<CustomerAddUpdateCompletedEventArgs> ExportCustomerAsyncTask(Loyalty.ServiceClient client, CustomerRequest request)
        //{
        //    var tcs = new TaskCompletionSource<CustomerAddUpdateCompletedEventArgs>();
        //    client.CustomerAddUpdateCompleted += (sender, e) => TransferCompletion(tcs, e, () => e);

        //    client.CustomerAddUpdateAsync(request);
        //    return tcs.Task;
        //}
    }
}
