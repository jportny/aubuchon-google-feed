using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Common.Logging;
using Mozu.Api;
using Mozu.Api.Contracts.Event;
using Mozu.Api.Contracts.MZDB;
using Mozu.Api.Resources.Platform.Entitylists;
using Mozu.Api.ToolKit.Config;

using Mozu.Api.ToolKit.Handlers;
using Mozu.Api.ToolKit.Readers;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Mozu.AubuchonDataAdapter.Domain.Events;
using Newtonsoft.Json.Linq;

namespace Mozu.AubuchonDataAdapter.Domain.Handlers
{
    public interface IEventHandler
    {
        Task InstallSchema(int tenantId);
        Task<bool> AddEvent(int tenantId, AubEvent aubEvent);
        Task<IEnumerable<AubEvent>> GetEvents(int tenantId, string filter = null);

        Task UpdateEvent(int tenantId, AubEvent aubEvent);
        Task DeleteEvent(IApiContext apiContext, string id);

        Task ProcessMissedEvents(IApiContext apiContext, DateTime? lastRunTime);

        Task PurgeOldEvents(IApiContext apiContext);

    }

    public class EventHandler : IEventHandler
    {
        private IApiContext _apiContext;
        private readonly IAppSetting _appSetting;
        private readonly string _listNameSpace;
        private readonly string _listFullName;
        ExceptionDispatchInfo _capturedException;
        private readonly IEventProcessor _eventProcessor;

        private readonly ILog _logger = LogManager.GetLogger(typeof(EventHandler));

        public EventHandler(IAppSetting appSetting, IEventProcessor eventProcessor)
        {
            _eventProcessor = eventProcessor;
            _appSetting = appSetting;
            _listNameSpace = MzdbHelper.GetListNameSpace(appSetting);
            _listFullName = MzdbHelper.GetListFullName(appSetting, EntityListConstants.MozuEventQueueName);
        }
        public async Task InstallSchema(int tenantId)
        {
            var correlationId = String.Empty;
            var errorCode = String.Empty;
            try
            {
                _apiContext = new ApiContext(tenantId);

                var eventQueueEntityList = new EntityList
                {
                    IsSandboxDataCloningSupported = false,
                    IsShopperSpecific = false,
                    IsVisibleInStorefront = false,
                    Name = EntityListConstants.MozuEventQueueName,
                    NameSpace = _listNameSpace
                };
                
                var idProperty = new IndexedProperty { DataType = "string", PropertyName = "Id" };

                var indexProperties = new List<IndexedProperty>
                {
                    new IndexedProperty { DataType = "string", PropertyName = "EventId"},
                    new IndexedProperty { DataType = "string", PropertyName = "EntityId"},
                    new IndexedProperty { DataType = "string", PropertyName = "Status"},
                    new IndexedProperty { DataType = "date", PropertyName = "QueuedDateTime"}
                };

                var entitySchemaHandler = new EntitySchemaHandler(_appSetting);
                
                await
                    entitySchemaHandler.InstallSchemaAsync(_apiContext, eventQueueEntityList, EntityScope.Tenant,
                        idProperty, indexProperties);
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
                    _logger.Error(message, _capturedException.SourceException);
                }
            }
        }

        public async Task<bool> AddEvent(int tenantId, AubEvent aubEvent)
        {
            if (!aubEvent.IsApproved()) return false;
            var correlationId = String.Empty;
            var errorCode = String.Empty;
            try
            {
                _apiContext = new ApiContext(tenantId);
                var entityResource = new EntityResource(_apiContext);

                var entity = await entityResource.GetEntityAsync(_listFullName, aubEvent.Id);
                if (entity == null)
                {
                //    var aubEvnt = entity.ToObject<AubEvent>();

                //    if (aubEvnt.Status == EventStatus.Processed.ToString()) return false;

                //    aubEvnt.Status = EventStatus.Pending.ToString();
                //    aubEvnt.QueuedDateTime = DateTime.UtcNow;
                //    await entityResource.UpdateEntityAsync(JObject.FromObject(aubEvnt), _listFullName, aubEvnt.Id);
                //}
                //else
                //{
                    await entityResource.InsertEntityAsync(JObject.FromObject(aubEvent), _listFullName);
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


        public async Task DeleteEvent(IApiContext apiContext, string id)
        {
            if (id == null) return;
            var correlationId = String.Empty;
            var errorCode = String.Empty;
            try
            {
                var resource = new EntityResource(apiContext);
                await resource.DeleteEntityAsync(_listFullName, id);
                //var entityHandler = new EntityHandler(_appSetting);
                //await entityHandler.DeleteEntityAsync(apiContext, id, EntityListConstants.MozuEventQueueName);
            }
            catch (AggregateException ex)
            {
                ex.Handle(e =>
                {
                    var apiEx = e as ApiException;
                    if (apiEx != null)
                    {
                        _capturedException = ExceptionDispatchInfo.Capture(apiEx);
                        correlationId = apiEx.CorrelationId;
                        errorCode = apiEx.ErrorCode;
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
            catch (Exception ex)
            {
                _capturedException = ExceptionDispatchInfo.Capture(ex);

            }
            if (_capturedException == null) return;
            if (_capturedException.SourceException.InnerException != null)
            {
                var message = String.Format("{0}. CorrId: {1}, ErrorCode: {2}",
                    _capturedException.SourceException.Message, correlationId,
                    errorCode);
                if (_capturedException.SourceException.InnerException != null)
                {
                    _logger.Error(message, _capturedException.SourceException);
                }
            }
        }

      
        public async Task UpdateEvent(int tenantId, AubEvent aubEvent)
        {
            try
            {
                _apiContext = new ApiContext(tenantId);

                var entityResource = new EntityResource(_apiContext);

                var entity = await entityResource.GetEntityAsync(_listFullName, aubEvent.Id);
                if (entity != null)
                {
                    await entityResource.UpdateEntityAsync(JObject.FromObject(aubEvent), _listFullName, aubEvent.Id);
                }
                else
                {
                    await AddEvent(tenantId, aubEvent);
                }
               
            }
            catch (ApiException ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public async Task<IEnumerable<AubEvent>> GetEvents(int tenantId, string filter = null)
        {
            var apiContext = new ApiContext(tenantId);
            var reader = new EntityReader<AubEvent>
            {
                Context = apiContext, 
                Filter = filter, 
                ListName = _listFullName, 
                Namespace = _listNameSpace,
                PageSize = 200
            };
            var entities = new List<AubEvent>();
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                entities.AddRange(reader.Items);
            }
            return entities;
        }

        public async Task ProcessMissedEvents(IApiContext apiContext, DateTime? lastRunTime)
        {
            //get events since last run
            var filter = string.Empty;
            if (lastRunTime.HasValue)
                filter = String.Format("createDate gt '{0}'", lastRunTime.Value.ToString("u"));

            var eventReader = new EventReader { Context = apiContext, Filter = filter, PageSize = 200};
            var eventList = new List<Event>();
            while (await eventReader.ReadAsync())
            {
                eventList.AddRange(eventReader.Items);
            }

            if (!eventList.Any()) return;

            var concise = from record in eventList
                group record by new {record.EntityId, record.Topic}
                into g
                let recent = (
                    from groupedItem in g
                    orderby groupedItem.AuditInfo.UpdateDate descending
                    select groupedItem
                    ).First()
                select recent;


            foreach (var evt in concise)
            {
                if (!evt.IsApproved()) continue;

                var aubEvent = evt.ToAubEvent();

                var entFilter = String.Format("Id eq '{0}' and Status eq 'Processed'", aubEvent.Id);
                var evtInEntity = await GetEvents(apiContext.TenantId, entFilter);
                var evtList = evtInEntity.ToList();

                if (evtList.Any())
                {
                    var ent = evtList.FirstOrDefault();
                    if (ent != null && ent.ProcessedDateTime > evt.AuditInfo.UpdateDate) continue;
                }

                var statusCode = StatusCode.New;
                if (evt.Topic.Contains(".updated"))
                {
                    statusCode = StatusCode.Active;
                }

                await _eventProcessor.ProcessEvent(apiContext, evt, statusCode);
            }
        }

        public async Task PurgeOldEvents(IApiContext apiContext)
        {
            var oldEvents =
                await
                    GetEvents(apiContext.TenantId,
                        String.Format("QueuedDateTime lt '{0}' and Status eq 'Processed'",
                            DateTime.UtcNow.AddDays(-2)));

            foreach (var item in oldEvents)
            {
                await DeleteEvent(apiContext, item.Id);
            }
        }

    }
}
