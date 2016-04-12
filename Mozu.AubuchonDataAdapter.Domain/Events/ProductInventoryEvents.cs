using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Mozu.Api;
using Mozu.Api.Contracts.Event;
using Mozu.Api.Contracts.ProductAdmin;
using Mozu.Api.Events;
using Mozu.Api.Resources.Commerce.Catalog.Admin.Attributedefinition.Producttypes;
using Mozu.Api.Resources.Commerce.Catalog.Admin.Products;
using Mozu.Api.ToolKit.Config;

namespace Mozu.AubuchonDataAdapter.Domain.Events
{
    public class ProductInventoryEvents : BaseProductEvent, IProductInventoryEvents
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(ProductInventoryEvents));
        public ProductInventoryEvents(IAppSetting appSetting)
            : base(appSetting)
        {

        }

        public void InStock(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public async Task InStockAsync(IApiContext apiContext, Event eventPayLoad)
        {
            try
            {
                var productCode = eventPayLoad.EntityId;
                _log.Info(String.Format("Inventory in stock for {0}", productCode));
                
                await ExportProductInventoryAsync(apiContext, eventPayLoad);

                var productAttribResource = new ProductPropertyResource(apiContext);
                var filter = String.Format("StockAvailable gt 0 and UpdateDate gt '{0}'",
                    DateTime.Now.AddMinutes(-5).ToString("u"));

                var locations = await GetProductLocations(apiContext, productCode, filter);


                //Add any new location to locationcode attribute

                var attributeResource = new Api.Resources.Commerce.Catalog.Admin.Attributedefinition.AttributeResource(apiContext);
                var locationCodeAttrib = await attributeResource.GetAttributeAsync(AttributeConstants.LocationCode);
                var hasNewLocationCode = false;

                foreach (var loc in locations.Where(loc => !locationCodeAttrib.VocabularyValues.Any(
                   a => a.Value != null && a.Content != null && Convert.ToString(a.Value).Equals(loc.LocationCode))))
                {
                    locationCodeAttrib.VocabularyValues.Add(new AttributeVocabularyValue { Value = loc.LocationCode });

                    hasNewLocationCode = true;
                }

                if (hasNewLocationCode)
                    await attributeResource.UpdateAttributeAsync(locationCodeAttrib, AttributeConstants.LocationCode);

                const int baseProductTypeId = 1;

                //Add attribute to product type
                var hasNewProductTypeValue = false;

                var productTypePropertyResource = new ProductTypePropertyResource(apiContext);

                var lcAttribute = await productTypePropertyResource.GetPropertyAsync(baseProductTypeId, AttributeConstants.LocationCode);

                foreach (
                     var loc in
                         locations)
                {
                    if (lcAttribute.VocabularyValues.Any(v => v.Value != null && Convert.ToString(v.Value).Equals(loc.LocationCode))) continue;

                    var val = new AttributeVocabularyValueInProductType
                    {
                        Value = loc.LocationCode,
                        Order = 0,
                        VocabularyValueDetail = new AttributeVocabularyValue { Value = loc.LocationCode, ValueSequence = 0, Content = new AttributeVocabularyValueLocalizedContent() { StringValue = loc.LocationCode } }
                    };

                    lcAttribute.VocabularyValues.Add(val);
                    hasNewProductTypeValue = true;
                }

                if (hasNewProductTypeValue)
                    await
                        productTypePropertyResource.UpdatePropertyAsync(lcAttribute, baseProductTypeId, lcAttribute.AttributeFQN);



                //Update Product Location Code attribute
                var locationAttrib = await productAttribResource.GetPropertyAsync(productCode, AttributeConstants.LocationCode);

                if (locationAttrib == null)
                {
                    foreach (var attr in locations.Select(location => new ProductProperty
                    {
                        AttributeFQN = locationCodeAttrib.AttributeFQN,
                        Values =
                            new List<ProductPropertyValue>
                            {
                                new ProductPropertyValue {Value = location.LocationCode}
                            }
                    }))
                    {
                        await productAttribResource.AddPropertyAsync(attr, productCode);
                    }
                }
                else
                {
                    locationAttrib.AttributeFQN = locationAttrib.AttributeFQN.ToLower();
                    var hasChanges = false;
                    foreach (var location in locations)
                    {
                        if (locationAttrib.Values != null && locationAttrib.Values.Any(a => a.Value.Equals(location.LocationCode))) continue;
                        if (locationAttrib.Values == null)
                        {
                            locationAttrib.Values = new List<ProductPropertyValue>();
                        }
                        locationAttrib.Values.Add(new ProductPropertyValue { Value = location.LocationCode });
                        hasChanges = true;
                    }
                    if (hasChanges)
                        await productAttribResource.UpdatePropertyAsync(locationAttrib, productCode, AttributeConstants.LocationCode);
                }
            }
            catch (Exception ex)
            {
                _log.Info(String.Format("Error occurred while processing in-stock notification for {0}", eventPayLoad.EntityId));
                _log.Error(String.Format("Error occurred while processing in-stock notification for {0}. Error: {1}", eventPayLoad.EntityId,ex.Message));
            }
        }

        public void OutOfStock(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public async Task OutOfStockAsync(IApiContext apiContext, Event eventPayLoad)
        {
            try
            {

                var productCode = eventPayLoad.EntityId;
               
                var productAttribResource = new ProductPropertyResource(apiContext);

                var filter = String.Format("StockAvailable le 0 and UpdateDate gt '{0}'",
                    DateTime.Now.AddMinutes(-5).ToString("u"));
                var locations = await GetProductLocations(apiContext, productCode, filter);

                var locationAttrib = await productAttribResource.GetPropertyAsync(productCode, AttributeConstants.LocationCode, "AttributeFQN,Values");
                if (locationAttrib.AttributeFQN != null)
                {
                    //locationAttrib.AttributeFQN = locationAttrib.AttributeFQN.ToLower();
                    var hasChanges = false;

                    foreach (
                      var item in
                          locations.Select(
                              location =>
                                  locationAttrib.Values.Where(p => p != null && Convert.ToString(p.Value).Equals(location.LocationCode, StringComparison.InvariantCultureIgnoreCase))).ToList()
                              .Where(item => item.Any()))
                    {
                        locationAttrib.Values.Remove(item.First());
                        hasChanges = true;
                    }

                    if (hasChanges)
                        await
                            productAttribResource.UpdatePropertyAsync(locationAttrib, productCode,
                                locationAttrib.AttributeFQN);
                }

                //await ExportProductInventoryAsync(apiContext, eventPayLoad);

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }

        }

        public void Updated(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task UpdatedAsync(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }
    }
}
