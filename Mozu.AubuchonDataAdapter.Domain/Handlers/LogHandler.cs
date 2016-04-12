using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Common.Logging;
using Mozu.Api;
using Mozu.Api.Contracts.MZDB;
using Mozu.Api.Resources.Platform.Entitylists;
using Mozu.Api.ToolKit.Config;
using Mozu.Api.ToolKit.Handlers;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Newtonsoft.Json.Linq;

namespace Mozu.AubuchonDataAdapter.Domain.Handlers
{
    public interface ILogHandler
    {
        Task InstallSchema(int tenantId);
        Task Log(int tenantId, AppLog appAppLog);
        Task<IEnumerable<AppLog>> List(int tenantId, int? pageSize, int? startIndex, string sortBy);
    }

    public class LogHandler : ILogHandler
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(LogHandler));
        private readonly IAppSetting _appSetting;
        private readonly string _listNameSpace;
        private readonly string _listFullName;
        ExceptionDispatchInfo _capturedException;

        public LogHandler(IAppSetting appSetting)
        {
            _appSetting = appSetting;
            _listNameSpace = MzdbHelper.GetListNameSpace(appSetting);
            _listFullName = MzdbHelper.GetListFullName(appSetting, EntityListConstants.AppLogListName);
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
                    IsVisibleInStorefront = true,
                    Name = EntityListConstants.AppLogListName,
                    NameSpace = _listNameSpace
                };

                var idProperty = new IndexedProperty { DataType = "string", PropertyName = "Id" };

                var indexProperties = new List<IndexedProperty>
                {
                    new IndexedProperty {DataType = "string", PropertyName = "EntityId"},
                    new IndexedProperty {DataType = "string", PropertyName = "EntityType"},
                    new IndexedProperty {DataType = "string", PropertyName = "LogType"},
                    new IndexedProperty {DataType = "date", PropertyName = "CreatedOn"}
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
                    _log.Error(message, _capturedException.SourceException);
                }
            }
        }

        public async Task Log(int tenantId, AppLog appAppLog)
        {
            var correlationId = String.Empty;
            var errorCode = String.Empty;
            try
            {
                var apiContext = new ApiContext(tenantId);
                var entityResource = new EntityResource(apiContext);
                await entityResource.InsertEntityAsync(JObject.FromObject(appAppLog), _listFullName);
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
                    _log.Error(message, _capturedException.SourceException);
                }
            }

        }

        public async Task<IEnumerable<AppLog>> List(int tenantId, int? pageSize, int? startIndex, string sortBy)
        {
            var apiContext = new ApiContext(tenantId);
            var entityResource = new EntityResource(apiContext);
            var entities = await entityResource.GetEntitiesAsync(_listFullName, pageSize, startIndex, sortBy: sortBy);
            return entities.Items.ConvertAll(ToAppLogConverter);
        }

        static AppLog ToAppLogConverter(JObject input)
        {
            return input.ToObject<AppLog>();
        }
    }


}
