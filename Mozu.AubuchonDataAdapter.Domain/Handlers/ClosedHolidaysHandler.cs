using Mozu.Api;
using Mozu.Api.Contracts.MZDB;
using Mozu.Api.Resources.Platform;
using Mozu.Api.Resources.Platform.Entitylists;
using Mozu.Api.ToolKit.Config;
using Mozu.Api.ToolKit.Handlers;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mozu.AubuchonDataAdapter.Domain.Handlers
{
    public interface IClosedHolidaysHandler
    {
        Task InstallClosedHolidaysSchemaAsync(int tenantId);
        Task<List<ClosedHoliday>> GetClosedHolidaysListAsinc(int tenantId);
        Task<bool> AddClosedHolidayAsync(int tenantId, ClosedHoliday closedHoliday);
        Task<bool> DeleteClosedHolidayAsync(int tenantId, ClosedHoliday closedHoliday);
    }

    public class ClosedHolidaysHandler : IClosedHolidaysHandler
    {
        #region Fields
        private readonly IAppSetting _appSetting;
        private readonly string _listNameSpace;
        private readonly string _listFullName;
        #endregion

        #region Constructor
        public ClosedHolidaysHandler(IAppSetting appSetting)
        {
            _appSetting = appSetting;
            _listNameSpace = MzdbHelper.GetListNameSpace(appSetting);
            _listFullName = MzdbHelper.GetListFullName(appSetting, EntityListConstants.ClosedHolidaysListName);
        }

        #endregion

        #region Helpers
        public async Task InstallClosedHolidaysSchemaAsync(int tenantId)
        {
            var apiContext = new ApiContext(tenantId);

            var handler = new EntityListResource(apiContext);
            var lists = await handler.GetEntityListsAsync().ConfigureAwait(false);
            if (lists.Items.Any(a => a.Name.Equals(EntityListConstants.ClosedHolidaysListName, StringComparison.CurrentCultureIgnoreCase))) return;

            var closedHolidaysEntityList = new EntityList
            {
                IsSandboxDataCloningSupported = false,
                IsShopperSpecific = false,
                IsVisibleInStorefront = true,
                Name = EntityListConstants.ClosedHolidaysListName,
                NameSpace = _listNameSpace
            };

            var idProperty = new IndexedProperty { DataType = "string", PropertyName = "Id" };
            var indexProperties = new List<IndexedProperty>
            {
                new IndexedProperty {DataType = "string", PropertyName = "HolidayDate"},
                new IndexedProperty {DataType = "string", PropertyName = "HolidayName"}
            };

            var entitySchemaHandler = new EntitySchemaHandler(_appSetting);
            await entitySchemaHandler.InstallSchemaAsync(apiContext, closedHolidaysEntityList, EntityScope.Tenant,
                idProperty, indexProperties).ConfigureAwait(false);
        }

        public async Task<List<ClosedHoliday>> GetClosedHolidaysListAsinc(int tenantId)
        {
            var apiContext = new ApiContext(tenantId);
            var entityResource = new EntityResource(apiContext);
            var entityResourceItems = await entityResource.GetEntitiesAsync(_listFullName);

            return entityResourceItems.Items.Select(item => item.ToObject<ClosedHoliday>()).ToList();
        }

        public async Task<bool> AddClosedHolidayAsync(int tenantId, ClosedHoliday closedHoliday)
        {
            var apiContext = new ApiContext(tenantId);
            var entityResource = new EntityResource(apiContext);

            var entityListResource = new Api.Resources.Platform.EntityListResource(apiContext);

            var list = await entityListResource.GetEntityListAsync(_listFullName);
            if (list == null)
                await InstallClosedHolidaysSchemaAsync(tenantId);

            var item = await entityResource.GetEntityAsync(_listFullName, closedHoliday.HolidayName);
            if (item != null)
                item = await entityResource.UpdateEntityAsync(JObject.FromObject(closedHoliday), _listFullName, closedHoliday.HolidayName);
            else
                item = await entityResource.InsertEntityAsync(JObject.FromObject(closedHoliday), _listFullName);
            return item != null;
        }

        public async Task<bool> DeleteClosedHolidayAsync(int tenantId, ClosedHoliday closedHoliday)
        {
            var apiContext = new ApiContext(tenantId);
            var entityResource = new EntityResource(apiContext);

            var item = await entityResource.GetEntityAsync(_listFullName, closedHoliday.Id);
            if (item == null) return false;
            await entityResource.DeleteEntityAsync(_listFullName, closedHoliday.Id);
            return true;
        } 
        #endregion
    }
}
