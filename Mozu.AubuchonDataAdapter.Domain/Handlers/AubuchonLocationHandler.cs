using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mozu.Api;
using Mozu.Api.Contracts.Location;
using System.Data;
using OfficeOpenXml;
using System.IO;
using Newtonsoft.Json.Linq;
using Mozu.Api.ToolKit.Readers;
using Mozu.AubuchonDataAdapter.Domain.Handlers.Excel;
using Mozu.Api.Contracts.MZDB;
using Mozu.Api.ToolKit.Handlers;
using Mozu.Api.ToolKit.Config;

namespace Mozu.AubuchonDataAdapter.Domain.Handlers
{
    public interface IAubuchonLocationHandler
    {
        Task ExportAubuchonLocations(int tenantId, string outputPath);
        Task ImportStoreExtras(int tenantId, string filePath, string detailsSheetName, string serviceSheetName);
        Task InstallLocationExtrasSchema(int tenantId);
        Task ImportLocationStoreDetails(int tenantId, string filePath, string worksheetName);
        Task ImportLocationExtraServices(int tenantId, string filePath, string worksheetName);

    }
    public class AubuchonLocationHandler : IAubuchonLocationHandler
    {
        #region Members
        private IApiContext _apiContext;
        private readonly IAppSetting _appSetting;
        #endregion

        #region Constructor
        public AubuchonLocationHandler(IAppSetting appSetting)
        {
            _appSetting = appSetting;
        }
        #endregion



        public async Task ImportStoreExtras(int tenantId, string filePath, string detailsSheetName, string serviceSheetName)
        {
            if (_apiContext == null)
                _apiContext = new ApiContext(tenantId);

            var storeDetails = Helper.GetDataTable(filePath, detailsSheetName);
            var serviceDetails = Helper.GetDataTable(filePath, serviceSheetName);

            if (storeDetails.Columns.Count == 0)
                return;
            //list to store neew entities
            var mzEntities = (from DataRow row in storeDetails.Rows
                              select new MzLocationExtra
                              {
                                  LocationCode = row["Mozu Location Code"].ToString().Trim(),
                                  ManagerName = row["Manager Name"].ToString(),
                                  ManagerImageUrl = row["Manager Image"].ToString(),
                                  StoreFrontImageUrl = row["Store Front Image"].ToString(),
                                  BDRName = row["Business Development Representative Name"].ToString(),
                                  BDRImageUrl = row["BDR Image"].ToString()
                              }).ToList();

            //loop through rows in excel sheet
            //release resources for excel datatables
            storeDetails.Dispose();

            var services = new List<string>();
            var locToService = new Dictionary<string, string[]>();

            foreach (DataRow item in serviceDetails.Rows)
            {

                services.Clear();
                if (String.IsNullOrWhiteSpace((string)item["Store #"]))
                    continue;

                for (int i = 2; i < item.ItemArray.Length; i++)
                {

                    var value = item.ItemArray[i];
                    if (value == DBNull.Value || (value is String && String.IsNullOrWhiteSpace((String)value)))
                        continue;
                    
                    services.Add(serviceDetails.Columns[i].ColumnName);
                }
                locToService.Add(((string)item["Store #"]).Trim(), services.ToArray());

            }
            serviceDetails.Dispose();


            var listname = MzdbHelper.GetListFullName(_appSetting, EntityListConstants.AubuchonLocationExtraListName);
            var entityListResource = new Api.Resources.Platform.EntityListResource(_apiContext);
            
            var list = await entityListResource.GetEntityListAsync(listname);
            if (list == null)
                await InstallLocationExtrasSchema(tenantId);

            var entityResource = new Api.Resources.Platform.Entitylists.EntityResource(_apiContext);
            foreach (var entity in mzEntities)
            {

                entity.Services = locToService.ContainsKey(entity.LocationCode) ? locToService[entity.LocationCode] : null;

                if (String.IsNullOrWhiteSpace(entity.LocationCode))
                    continue;

                var existingItem = await
                    entityResource.GetEntityAsync(listname, entity.LocationCode);

                if (existingItem != null)
                {
                    await entityResource.UpdateEntityAsync(JObject.FromObject(entity), listname, entity.LocationCode);
                }
                else
                {
                    await entityResource.InsertEntityAsync(JObject.FromObject(entity), listname);
                }

            }
        }

        #region Import Details / Import Extra Services
        public async Task ImportLocationStoreDetails(int tenantId, string filePath, string worksheetName)
        {
            if (_apiContext == null)
                _apiContext = new ApiContext(tenantId);

            var storeDetails = Helper.GetDataTable(filePath, worksheetName);

            if (storeDetails.Columns.Count == 0)
                return;
            //list to store neew entities
            var mzEntities = (from DataRow row in storeDetails.Rows
                              select new MzLocationExtra
                              {
                                  LocationCode = row["Mozu Location Code"].ToString().Trim(),
                                  ManagerName = row["Manager Name"].ToString(),
                                  ManagerImageUrl = row["Manager Image"].ToString(),
                                  StoreFrontImageUrl = row["Store Front Image"].ToString(),
                                  BDRName = row["Business Development Representative Name"].ToString(),
                                  BDRImageUrl = row["BDR Image"].ToString()
                              }).ToList();

            //loop through rows in excel sheet
            //release resources for excel datatables
            storeDetails.Dispose();
            var listname = MzdbHelper.GetListFullName(_appSetting, EntityListConstants.AubuchonLocationExtraListName);
            var entityResource = new Api.Resources.Platform.Entitylists.EntityResource(_apiContext);

            foreach (var entity in mzEntities.Where(entity => !String.IsNullOrWhiteSpace(entity.LocationCode)))
            {
                var existingItem =
                    await entityResource.GetEntityAsync(listname, entity.LocationCode);

                if (existingItem != null)
                {
                    await entityResource.UpdateEntityAsync(JObject.FromObject(entity), listname, entity.LocationCode);
                }
                else
                {
                    await entityResource.InsertEntityAsync(JObject.FromObject(entity), listname);
                }
            }
        }

        public async Task ImportLocationExtraServices(int tenantId, string filePath, string worksheetName)
        {
            if (_apiContext == null)
                _apiContext = new ApiContext(tenantId);

            DataTable data = Helper.GetDataTable(filePath, worksheetName);

            List<string> services = new List<string>();
            Dictionary<string, string[]> locToService = new Dictionary<string, string[]>();

            foreach (DataRow item in data.Rows)
            {

                services.Clear();
                if (String.IsNullOrWhiteSpace((string)item["Store #"]))
                    continue;

                for (int i = 2; i < item.ItemArray.Length; i++)
                {

                    var value = item.ItemArray[i];
                    if (value == DBNull.Value)
                        continue;

                    services.Add(data.Columns[i].ColumnName);
                }
                locToService.Add(((string)item["Store #"]).Trim(), services.ToArray());

            }
            data.Dispose();

            var entityResource = new EntityHandler(_appSetting);

            string listname = MzdbHelper.GetListFullName(_appSetting, EntityListConstants.AubuchonLocationExtraListName);

            foreach (var entry in locToService)
            {
                var mzLoc = entityResource.GetEntityAsync<MzLocationExtra>(_apiContext, entry.Key, listname).Result;

                if (mzLoc == null)
                {
                    mzLoc = new MzLocationExtra { LocationCode = entry.Key, Services = entry.Value };
                    await entityResource.AddEntityAsync(_apiContext, listname, mzLoc);
                }
                else
                {
                    mzLoc.Services = entry.Value;
                    await entityResource.UpdateEntityAsync(_apiContext, entry.Key, listname, mzLoc);
                }
            }


        }
        #endregion

        #region InstallMZDBLocationSchema
        public async Task InstallLocationExtrasSchema(int tenantId)
        {
            if (_apiContext == null)
                _apiContext = new ApiContext(tenantId);

            var entitySchemaHandler = new EntitySchemaHandler(_appSetting);
            var listNs = MzdbHelper.GetListNameSpace(_appSetting);
               
            var locationExtraList = new EntityList
            {
                IsSandboxDataCloningSupported = true,
                IsVisibleInStorefront = true,
                Name = EntityListConstants.AubuchonLocationExtraListName,
                NameSpace = listNs
            };

            var idProperty = new IndexedProperty { DataType = "string", PropertyName = "LocationCode" };

            await entitySchemaHandler
                .InstallSchemaAsync(_apiContext, locationExtraList, EntityScope.Tenant, idProperty, null)
                .ConfigureAwait(false);







            /*
                               string _listFullName = MzdbHelper.GetListFullName(_appSetting, EntityListConstants.AubuchonLocationExtraListName);
                               string _listNS = MzdbHelper.GetListNameSpace(_appSetting);
                               var entitySchemaHandler = new EntitySchemaHandler(_appSetting);

                                   var locationExtraList = new EntityList()
                                   {
                                       IsSandboxDataCloningSupported = true,
                                       IsVisibleInStorefront = true,
                                       Name = _listFullName,
                                       NameSpace = _listNS
                                   };
               
                                   var schemaHandler = new Mozu.Api.Resources.Platform.EntityListResource(_apiContext);
                                  var result = schemaHandler.CreateEntityListAsync(locationExtraList).Result;
                    
                                   result.IdProperty = idProperty;
                               s
                               */
            /*
                _listFullName = MzdbHelper.GetListFullName(_appSetting, EntityListConstants.AubuchonLocationServices);

                    var locationServicesList = new EntityList()
                    {
                        IsVisibleInStorefront = true,
                        Name = _listFullName,
                        NameSpace = _listNS
                    };
                    var serviceIdProperty = new IndexedProperty { DataType = "string", PropertyName = "Service" };
                    
                    entitySchemaHandler
                        .InstallSchemaAsync(_apiContext, locationServicesList, EntityScope.Tenant, serviceIdProperty, null)
                            .ConfigureAwait(false);
                */
            /*
                    var idProperty = new IndexedProperty { DataType = "string", PropertyName = "LocationCode" };
                    var result = entitySchemaHandler.InstallSchemaAsync(_apiContext, locationExtraList, EntityScope.Tenant, idProperty, null).Result;*/
        }
        #endregion

        #region ExportAubuchonLocationExtras

        /// <summary>
        /// Using Spreadsheetlight for easy excel creation. MIT License, on Nuget.
        /// </summary>
        public async Task ExportAubuchonLocations(int tenantId, string outputPath)
        {
            _apiContext = new ApiContext(tenantId);

            var fileName = "ExportLocation" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            var file = new FileInfo(outputPath + fileName);

            if (file.Exists)
                file.Delete();

            var entityResource = new Api.Resources.Platform.Entitylists.EntityResource(_apiContext);

            string listname = MzdbHelper.GetListFullName(_appSetting, EntityListConstants.AubuchonLocationExtraListName);

            var locationReader = new LocationReader
            {
                Context = _apiContext,
                PageSize = 200
            };

            var rowNumber = 1;
            using (var package = new ExcelPackage(file))
            {
                //store details worksheet
                ExcelWorksheet detailsWorksheet = package.Workbook.Worksheets.Add(Helper.STORE_DETAILS_WS_NAME);

                ExcelWorksheet servicesWorksheet = package.Workbook.Worksheets.Add(Helper.STORE_SERVICES_WS_NAME);
                //add details Headers
                for (var i = 0; i < Helper.STORE_DETAILS_HEADER_XL.Length; i++)
                    detailsWorksheet.Cells[rowNumber, i + 1].Value = Helper.STORE_DETAILS_HEADER_XL[i];

                //add service headers
                ExcelHeader[] serviceHeaders = Helper.GetHeaders("locationservicesheaders.json").ToArray();
                for (var i = 0; i < serviceHeaders.Length; i++)
                    servicesWorksheet.Cells[rowNumber, i + 1].Value = serviceHeaders[i].ColumnName;

                var serviceDictionary = new Dictionary<string, int>();
                for (int i = 0; i < serviceHeaders.Length; i++)
                {
                    serviceDictionary.Add(serviceHeaders[i].ColumnName, i + 1);
                }

                //freeze headers
                detailsWorksheet.View.FreezePanes(2, 2);

                //move row down.
                rowNumber++;
                int servicesRow = 2;

                while (await locationReader.ReadAsync())
                {

                    foreach (var loc in locationReader.Items.OrderBy(l => l.Code))
                    {
                        var entity = await entityResource.GetEntityAsync(listname, loc.Code);

                        var mzLocation = entity == null ? new MzLocationExtra() : entity.ToObject<MzLocationExtra>();
                        servicesWorksheet.Cells[servicesRow, 1].Value = loc.Code;
                        servicesWorksheet.Cells[servicesRow, 2].Value = loc.Name;

                        if (mzLocation.Services != null && mzLocation.Services.Length != 0)
                        {
                            foreach (var service in mzLocation.Services)
                            {
                                servicesWorksheet.Cells[servicesRow, _getServiceColumnHeaderValue(service, serviceDictionary)].Value = "X";
                            }
                        }


                        var headerIndex = 0;
                        for (var columnNumber = 1; columnNumber < Helper.STORE_DETAILS_HEADER_XL.Length; columnNumber++)
                        {
                            try
                            {
                                detailsWorksheet.Cells[rowNumber, columnNumber].Value = _getDetailsCellValue(Helper.STORE_DETAILS_HEADER_XL[headerIndex], loc, mzLocation);

                                headerIndex++;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                break;
                            }

                        }
                        rowNumber++;
                        servicesRow++;
                    }

                    detailsWorksheet.Column(1).Style.Locked = true;
                    detailsWorksheet.Cells[detailsWorksheet.Dimension.Address].AutoFitColumns();
                    servicesWorksheet.Cells[servicesWorksheet.Dimension.Address].AutoFitColumns();
                }

                package.Save();
                package.Dispose();
            }

        }
        private int _getServiceColumnHeaderValue(string service, Dictionary<string, int> headers)
        {
            return headers.ContainsKey(service) ? headers[service] : 0;
        }
        private string _getDetailsCellValue(string header, Location location, MzLocationExtra mzLocation)
        {

            switch (header)
            {
                case "Mozu Location Code":
                    return location.Code;
                case "Location Name":
                    return location.Name;
                case "Manager Name":
                    return mzLocation.ManagerName;
                case "Manager Image":
                    return mzLocation.ManagerImageUrl;
                case "Store Front Image":
                    return mzLocation.StoreFrontImageUrl;
                case "Business Development Representative Name":
                    return mzLocation.BDRName;
                case "BDR Image":
                    return mzLocation.BDRImageUrl;
                default:
                    return String.Empty;

            }
        }
        #endregion

    }
}

