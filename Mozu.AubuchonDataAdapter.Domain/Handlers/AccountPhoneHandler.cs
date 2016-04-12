using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Common.Logging;
using Mozu.Api;
using Mozu.Api.Contracts.MZDB;
using Mozu.Api.Resources.Commerce.Customer;
using Mozu.Api.Resources.Platform.Entitylists;
using Mozu.Api.ToolKit.Config;
using Mozu.Api.ToolKit.Handlers;
using Mozu.Api.ToolKit.Readers;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Newtonsoft.Json.Linq;

namespace Mozu.AubuchonDataAdapter.Domain.Handlers
{
    public interface IAccountPhoneHandler
    {
        Task InstallAccountPhoneSchema(int tenantId);
        Task<bool> UpsertAccountPhoneAsync(int tenantId, AccountPhone accountPhone);
        Task<IList<AccountPhone>> GetAccountsWithPhone(int tenantId, string phoneNumber);
    }

    public class AccountPhoneHandler : IAccountPhoneHandler
    {
        private readonly IAppSetting _appSetting;
        private readonly string _listNameSpace;
        private readonly string _listFullName;
        ExceptionDispatchInfo _capturedException;
        private readonly ILog _logger = LogManager.GetLogger(typeof(AccountPhoneHandler));
        public AccountPhoneHandler(IAppSetting appSetting)
        {
            _appSetting = appSetting;
            _listNameSpace = MzdbHelper.GetListNameSpace(appSetting);
            _listFullName = MzdbHelper.GetListFullName(appSetting, EntityListConstants.AccountPhoneListName);
        }
        public async Task InstallAccountPhoneSchema(int tenantId)
        {
            var apiContext = new ApiContext(tenantId);

            var accountPhoneEntityList = new EntityList
            {
                IsSandboxDataCloningSupported = false,
                IsShopperSpecific = false,
                IsVisibleInStorefront = true,
                Name = EntityListConstants.AccountPhoneListName,
                NameSpace = _listNameSpace
            };

            var idProperty = new IndexedProperty { DataType = "string", PropertyName = "Id" };

            var indexProperties = new List<IndexedProperty>
                {
                    new IndexedProperty {DataType = "integer", PropertyName = "AccountId"},
                    new IndexedProperty {DataType = "string", PropertyName = "Phone"}
                };

            var entitySchemaHandler = new EntitySchemaHandler(_appSetting);
            await
                entitySchemaHandler.InstallSchemaAsync(apiContext, accountPhoneEntityList, EntityScope.Tenant,
                    idProperty, indexProperties).ConfigureAwait(false);
        }

        public async Task<bool> UpsertAccountPhoneAsync(int tenantId, AccountPhone accountPhone)
        {
            var correlationId = String.Empty;
            var errorCode = String.Empty;
            try
            {
                var apiContext = new ApiContext(tenantId);
                var entityResource = new EntityResource(apiContext);
                accountPhone.Phone = StripPhone(accountPhone.Phone);

                var entry = await entityResource.GetEntityAsync(_listFullName, accountPhone.Id);
                if (entry == null)
                {
                    var response =
                        await
                            entityResource.InsertEntityAsync(JObject.FromObject(accountPhone), _listFullName)
                                .ConfigureAwait(false);
                    return response != null && response.ToObject<AccountPhone>().Id != null;
                }
                var result =
                    await
                        entityResource.UpdateEntityAsync(JObject.FromObject(accountPhone), _listFullName,
                            accountPhone.Id).ConfigureAwait(false);
                if (result != null)
                {
                    return result.ToObject<AccountPhone>().Id != null;
                }
            }
            catch (AggregateException ex)
            {
                ex.Handle(e =>
                {
                    var apiEx = e as ApiException;
                    if (apiEx != null)
                    {
                        if (apiEx.ErrorCode.Trim() == "ITEM_ALREADY_EXISTS") return true;
                        _capturedException = ExceptionDispatchInfo.Capture(apiEx);
                        correlationId = apiEx.CorrelationId;
                        errorCode = apiEx.ErrorCode;
                    }
                    else
                    {
                        _capturedException = ExceptionDispatchInfo.Capture(e);
                    }
                    return true;
                });

            }
            catch (ApiException ex)
            {
                if (ex.ErrorCode.Trim() == "ITEM_ALREADY_EXISTS") return false;
                _capturedException = ExceptionDispatchInfo.Capture(ex);
                correlationId = ex.CorrelationId;
                errorCode = ex.ErrorCode;
            }
            catch (Exception ex)
            {
                _capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            if (_capturedException != null && _capturedException.SourceException.InnerException != null)
            {
                var message = String.Format("{0}. CorrId: {1}, ErrorCode: {2}",
                    _capturedException.SourceException.Message, correlationId,
                    errorCode);
                if (_capturedException.SourceException.InnerException != null)
                {
                    _logger.Error(message, _capturedException.SourceException);
                }
            }
            return false;
        }

        public async Task<IList<AccountPhone>> GetAccountsWithPhone(int tenantId, string phoneNumber)
        {
            var apiContext = new ApiContext(tenantId);
            var resource = new CustomerAccountResource(apiContext);
            var entityResource = new EntityResource(apiContext);
            
            var reader = new EntityReader<AccountPhone>
            {
                Context = apiContext,
                Filter = String.Format("Phone eq '{0}' OR Phone eq '{1}'", phoneNumber, StripPhone(phoneNumber)),
                ListName = _listFullName,
                PageSize = 200
            };
            var preliveCustomers = new List<AccountPhone>();
            while (await reader.ReadAsync())
            {
                await reader.Items.ForEachAsync(10, async item =>
                {
                    var account =
                        await resource.GetAccountAsync(item.AccountId, "Id, Attributes(fullyQualifiedName,values) ");
                    if (account == null)
                    {
                        await entityResource.DeleteEntityAsync(_listFullName, item.Id);
                        return;
                    }
                    //if (account.IsPreLiveCustomer())
                    //{
                        preliveCustomers.Add(item);
                    //}
                });
            }
            return preliveCustomers;
        }

        static string StripPhone(string phoneNumber)
        {
            return !String.IsNullOrEmpty(phoneNumber) ? phoneNumber.Replace(" ", "").Replace("-", "") : String.Empty;
        }
    }
}
