using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mozu.Api;
using Mozu.Api.Contracts.Event;
using Mozu.Api.Contracts.ProductAdmin;
using Mozu.Api.Resources.Commerce.Catalog.Storefront;
using Mozu.Api.Resources.Content.Documentlists;
using Mozu.Api.ToolKit.Config;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
using Mozu.AubuchonDataAdapter.Domain.Utility;

namespace Mozu.AubuchonDataAdapter.Domain.Events
{
    public abstract class BaseProductEvent
    {
        private readonly IAppSetting _appSetting;
        protected BaseProductEvent(IAppSetting appSetting)
        {
            _appSetting = appSetting;
        }


        protected async Task<IList<LocationInventory>> GetProductLocations(IApiContext apiContext, string productCode, string filter = null)
        {
            var locations = new List<LocationInventory>();

            var inventoryReader = new ProductLocationInventoryReader
            {
                Context = apiContext,
                ProductCode = productCode,
                PageSize = 200,
                ResponseFields = "Items(LocationCode),AuditInfo",
                Filter = filter
            };
            while (await inventoryReader.ReadAsync())
            {
                locations.AddRange(inventoryReader.Items);
            }
            return locations;
        }

        protected async Task ExportProductInventoryAsync(IApiContext apiContext, Event eventPayLoad)
        {
            var productHandler = new ProductExportHandler(_appSetting);
            var product = await productHandler.GetProduct(apiContext, eventPayLoad.EntityId);
            var locations = await GetProductLocations(apiContext, product.ProductCode);
            await productHandler.ExportInventoryAsync(apiContext, product, locations);

        }
       protected async Task ExportProductAsync(IApiContext apiContext, Event eventPayLoad)
        {
            var productHandler = new ProductExportHandler(_appSetting);
            var product = await productHandler.GetProduct(apiContext, eventPayLoad.EntityId);



            var productStoreResource = new ProductResource(apiContext);
            var sfProd = await productStoreResource.GetProductAsync(product.ProductCode, responseFields: "Content(ProductImages)");

            if (sfProd != null)
            {
                //Get images form storefront product
                var images = sfProd.Content.ProductImages;
                var productImages = new List<ProductLocalizedImage>();
                if (images != null)
                {
                    var documentResource = new DocumentResource(apiContext);
                    foreach (var image in images)
                    {
                        var imageUrl = image.ImageUrl ??
                                       String.Format("//cdn-sb.mozu.com/{0}-m1/cms/files/{1}", apiContext.TenantId,
                                           image.CmsId);

                        var document = await documentResource.GetDocumentAsync("files@mozu", image.CmsId);
                        productImages.Add(new ProductLocalizedImage
                        {

                            CmsId = image.CmsId,
                            ImageLabel = document.Name,
                            //ImageUrl = image.ImageUrl.Replace(image.CmsId, document.Name)
                            ImageUrl = imageUrl.Replace(image.CmsId, document.Name)
                        });
                    }
                    product.Content.ProductImages = productImages;
                }

                await productHandler.ExportFileuploadAsync(productImages);
            }

            //Get Locations for Product
           var locations = await GetProductLocations(apiContext, product.ProductCode);


            await productHandler.ExportInventoryAsync(apiContext, product, locations);
            await productHandler.ExportProductAsync(apiContext,product);
        }

    }
}
