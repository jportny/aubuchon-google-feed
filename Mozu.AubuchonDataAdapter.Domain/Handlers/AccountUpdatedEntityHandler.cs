using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Autofac;
using Mozu.Api;
using Mozu.Api.Contracts.MZDB;
using Mozu.Api.ToolKit.Config;
using Mozu.Api.ToolKit.Handlers;

namespace Mozu.AubuchonDataAdapter.Domain.Handlers
{
    public class AccountUpdatedEntityHandler
    {
        private readonly IAppSetting _appSetting;
        private readonly string _listNameSpace;
        private readonly string _listFullName;
        ExceptionDispatchInfo _capturedException;
        private readonly IComponentContext _componentContext;

        public AccountUpdatedEntityHandler(IAppSetting appSetting ,IComponentContext componentContext)
        {
            _appSetting = appSetting;
            _listNameSpace = MzdbHelper.GetListNameSpace(appSetting);
            _listFullName = MzdbHelper.GetListFullName(appSetting, EntityListConstants.MozuEventQueueName);
            _componentContext = componentContext;
        }

        public async Task InstallSchema(int tenantId)
        {
            var correlationId = String.Empty;
            var errorCode = String.Empty;
            try
            {
                var apiContext = new ApiContext(tenantId);

                var errorQueueEntityList = new EntityList
                {
                    IsSandboxDataCloningSupported = false,
                    IsShopperSpecific = false,
                    IsVisibleInStorefront = false,
                    Name = EntityListConstants.MozuEventQueueName,
                    NameSpace = _listNameSpace
                };

                var idProperty = new IndexedProperty { DataType = "integer", PropertyName = "AccountId" };

                var indexProperties = new List<IndexedProperty>
                {
                    new IndexedProperty {DataType = "date", PropertyName = "UpdatedDateTime"}
                };

                var entitySchemaHandler = new EntitySchemaHandler(_appSetting);
                await
                    entitySchemaHandler.InstallSchemaAsync(apiContext, errorQueueEntityList, EntityScope.Tenant,
                        idProperty, indexProperties).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                ex.Flatten().Handle(e =>
                {
                    var apiEx = e as ApiException;
                    if (apiEx != null)
                    {
                        _capturedException = ExceptionDispatchInfo.Capture(apiEx);
                        correlationId = apiEx.CorrelationId;
                        errorCode = apiEx.ErrorCode;
                    }
                    else if (e is HttpRequestException || e is TaskCanceledException)
                    {
                        _capturedException = ExceptionDispatchInfo.Capture(ex);
                    }
                    else
                    {
                        _capturedException = ExceptionDispatchInfo.Capture(ex);
                    }
                    return true;
                });
            }
            catch (ApiException ex)
            {
                _capturedException = ExceptionDispatchInfo.Capture(ex);
                correlationId = ex.CorrelationId;
                errorCode = ex.ErrorCode;
            }
            catch (HttpRequestException ex)
            {
                _capturedException = ExceptionDispatchInfo.Capture(ex);
            }
            catch (TaskCanceledException ex)
            {
                _capturedException = ExceptionDispatchInfo.Capture(ex);
            }
            catch (Exception ex)
            {
                _capturedException = ExceptionDispatchInfo.Capture(ex);
            }
            if (_capturedException != null)
            {
                var message = String.Format("{0}. CorrId: {1}, ErrorCode: {2}",
                    _capturedException.SourceException.Message, correlationId,
                    errorCode);
                if (_capturedException.SourceException.InnerException != null)
                {
                    //Log it
                    //_log.Error(message, _capturedException.SourceException).Wait();
                }
            }
        }

        //public async Task CheckAndReprocessEvents(IApiContext apiContext, DateTime? lastRunTime)
        //{
        //    var filter = string.Empty;
        //    if (lastRunTime.HasValue)
        //        filter = "createDateTime gt " + lastRunTime.Value.ToString("u");

        //    var eventReader = new EventReader {Context = apiContext, Filter = filter};
        //    while (await eventReader.ReadAsync())
        //    {

        //        foreach (var evt in eventReader.Items)
        //        {
        //            //re-process event
        //            await ProcessEvent(apiContext, evt);
        //        }
        //    }

        //    //var items = new List<AccountUpdated>();
        //    //var entityReader = new EntityReader<AccountUpdated>
        //    //{
        //    //    Context = apiContext,
        //    //    Filter = String.Format("UpdatedDateTime gt '{0}'", updateDateTime.ToUniversalTime()),
        //    //    ListName = _listFullName,
        //    //    PageSize = 200
        //    //};

        //    //while (await entityReader.ReadAsync())
        //    //{
        //    //    items.AddRange(entityReader.Items);
        //    //}
        //    //return items;
        //}

        //public async Task ProcessEvent(IApiContext apiContext, AubEvent evt)
        //{
        //    try
        //    {
        //        var eventProcessor = _componentContext.ResolveNamed<IEventProcessor>(evt.Topic);
        //        await eventProcessor.ProcessEvent(apiContext, evt);
        //        //await _entityHandler.UpsertEntityAsync(apiContext, evt.Id, SchemaNames.EVENT, evt);
        //    }
        //    catch (Exception exc)
        //    {
        //        //_logger.Error(exc);
        //        throw;
        //    }
        //}
    }
}
