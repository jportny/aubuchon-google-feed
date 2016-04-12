using System.Collections.Generic;
using System.Threading.Tasks;
using Mozu.Api.Contracts.ProductAdmin;
using Mozu.Api.ToolKit.Readers;

namespace Mozu.AubuchonDataAdapter.Domain.Utility
{
    public class ProductLocationInventoryReader : BaseReader
    {
        private LocationInventoryCollection _results;

        public string ProductCode { get; set; }

        public List<LocationInventory> Items
        {
            get
            {
                return _results.Items;
            }
        }

        protected override async Task<bool> GetDataAsync()
        {
            var resource = new Api.Resources.Commerce.Catalog.Admin.Products.LocationInventoryResource(Context);
            _results = await resource.GetLocationInventoriesAsync(ProductCode, StartIndex, PageSize, SortBy, Filter, ResponseFields);
           
            TotalCount = _results.TotalCount;
            PageCount = _results.PageCount;
            PageSize = _results.PageSize;
            return _results.Items != null && _results.Items.Count > 0;
        }
    }
}
