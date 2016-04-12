using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mozu.Api.Contracts.ProductAdmin;
using Mozu.AubuchonDataAdapter.Domain.Contracts;

namespace Mozu.AubuchonDataAdapter.Domain.Mappers
{
    public static class InventoryMapper
    {
        public static InventoryMessage ToEdgeInventoryExportMessage(this Product product, string providerCode,
            string xrefMerchantId, IList<LocationInventory> locations)
        {
            var message = new InventoryMessage();

            var head = new MessageHead
            {
                Sender = new MessageHeadSender[1],
                Recipient = new MessageHeadRecipient[1],
                Messageid = Guid.NewGuid().ToString(),
                Messagetype = "Inventoryimport",
                Date = DateTime.Now.ToString(CultureInfo.InvariantCulture)
            };
            var sender = new MessageHeadSender { Systemid = providerCode };
            head.Sender[0] = sender;

            var recipient = new MessageHeadRecipient { Companyid = xrefMerchantId };
            head.Recipient[0] = recipient;
            message.Items = new object[2];
            message.Items[0] = head;

            var status = StatusCode.Active;
            var prodInCatatlog = product.ProductInCatalogs.FirstOrDefault();
            if (prodInCatatlog != null)
            {
                status = prodInCatatlog.IsActive.GetValueOrDefault() ? StatusCode.Active : StatusCode.Inactive;
            }

            var body = new InventoryMessageBody { Inventoryimport = new MessageBodyInventoryimport[1] };

            var inventoryImport = new MessageBodyInventoryimport
            {
                Sku = product.ProductCode,
                Description = product.Content.ProductName,
                Skutypecode = "Standard",
                Manufacturername = product.GetPropertyStringLabel(AttributeConstants.Brand) ?? String.Empty,
                Uomname = product.GetPropertyStringLabel(AttributeConstants.UnitOfMeasure) ?? String.Empty,
                Manufacturerpartno = product.SupplierInfo.MfgPartNumber ?? String.Empty,//TODO:Check the actual mapping
                Width =  product.PackageWidth != null ? Convert.ToString(product.PackageWidth.Value) : "0.1",
                Cost = product.SupplierInfo.Cost.Cost.ToString(),
                Reorderpoint = String.Empty,
                Maxbuildqty = String.Empty,
                Qtyperpackage = String.Empty,
                Leadtime = String.Empty,
                Allowbackorder = product.InventoryInfo.OutOfStockBehavior == "AllowBackOrder" ? "1" : "0",
                Statuscode = Enum.GetName(typeof(StatusCode), status),
                Statusdate = String.Empty,
                Custodianusername = "defaultAdmin",
                Parentsku = String.Empty,
                Inventory = "1",
                Returnable = "1",
                Refundable = "1",
                Restockable = "1",
                Obsolete = "0",
                Weight = product.PackageWeight != null ?  Convert.ToString(product.PackageWeight.Value) : "0.1",
                Length = product.PackageLength != null ? Convert.ToString(product.PackageLength.Value) : "0.1",
                Height = product.PackageHeight != null ? Convert.ToString(product.PackageHeight.Value) : "0.1",
                Lwhunit = String.Empty,
                Upcnumber = product.Upc ?? String.Empty,//TODO:Check what should be the value if null
                Trademarkname = String.Empty,
                Legacynumber = String.Empty,
                Hazardclass = product.GetPropertyBooleanValue(AttributeConstants.Hazardous) ? "1" : "0",
                Packaginggroup = String.Empty,
                Colorname = String.Empty,
                Sizename = String.Empty,
                Stylename = String.Empty,
                InventoryNotes = String.Empty,
                Skuplacementcode = String.Empty,
                Forecasteddate = String.Empty,
                Reorderrulecode = String.Empty,
                Reorderqty = String.Empty,
                Avgdailyusage = String.Empty,
                Secureshipment = String.Empty,
                Graceperiod = String.Empty,
                Docktostock = String.Empty,
                Minbuildqty = String.Empty,
                Startdate = DateTime.Today.ToString("yyyy-MM-dd"),
                Enddate = String.Empty,
                Categories = new MessageBodyInventoryimportCategoriesCategoryCode[] { },
                Providers = new MessageBodyInventoryimportProviders(),
                Suppliers = new MessageBodyInventoryimportSuppliers[] {  },
                Projects = new[] { new MessageBodyInventoryimportProjectsProject { Value = "Aubuchon" } },
                Locales = new[] { new MessageBodyInventoryimportLocalesLocale
                {
                    LocaleCode = "en-US", 
                    Description = product.Content.ProductName, 
                    InventoryNotes = product.Content.ProductShortDescription, 
                    RetailPrice = Convert.ToString(product.Price.Price), 
                    SalePrice = product.Price.SalePrice == null ? Convert.ToString(product.Price.Price) : Convert.ToString(product.Price.SalePrice), 
                    Cost = Convert.ToString(product.SupplierInfo.Cost.Cost)
                    
                } },
                Dynamicfields = new MessageBodyInventoryimportDynamicfieldsDynamicfield[] { },
                
            };
            
            if (product.Content.ProductImages != null)
            {
                inventoryImport.Filenames =
                    new MessageBodyInventoryimportFilenamesFilename[product.Content.ProductImages.Count];

                for (var i = 0; product.Content.ProductImages != null && i < product.Content.ProductImages.Count; i++)
                {
                    inventoryImport.Filenames[i] = new MessageBodyInventoryimportFilenamesFilename
                    {
                        Default = i == 0 ? "1" : "0",
                        Value = product.Content.ProductImages[i].ImageLabel
                    };
                     
                }
            }

            if (locations != null)
            {

                inventoryImport.Providers = new MessageBodyInventoryimportProviders
                {
                    ProviderCode = new MessageBodyInventoryimportProvidersProviderCode[locations.Count]
                };
                for (var i = 0; i < locations.Count; i++)
                {
                    inventoryImport.Providers.ProviderCode[i]= new MessageBodyInventoryimportProvidersProviderCode
                    {
                        ProviderCode = locations[i].LocationCode
                    };
                }
            }

            //inventoryImport.Suppliers = new MessageBodyInventoryimportSuppliers[1];
            //inventoryImport.Suppliers[0] = new MessageBodyInventoryimportSuppliers();

            //inventoryImport.Projects = new MessageBodyInventoryimportProjectsProject[1];
            //inventoryImport.Projects[0] = new MessageBodyInventoryimportProjectsProject();

            //inventoryImport.Locales = new MessageBodyInventoryimportLocalesLocale[1];

            //inventoryImport.Locales[0] = new MessageBodyInventoryimportLocalesLocale
            //{
            //    LocaleCode = "US-EN",
            //    Description = product.Content.ProductShortDescription,
            //    InventoryNotes = "test note",//TODO: Check the right mapping value as it is required
            //    RetailPrice = Convert.ToString(product.Price.Price),
            //    SalePrice = Convert.ToString(product.Price.SalePrice ?? product.Price.Price) //TODO:Check what should be passed if SalePrice is null
            //};

            //inventoryImport.Dynamicfields = new MessageBodyInventoryimportDynamicfieldsDynamicfield[1];
            body.Inventoryimport[0] = inventoryImport;
            message.Items[1] = body;

            return message;
        }
    }
}
