using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mozu.Api;
using Mozu.Api.Contracts.ProductAdmin;
using Mozu.Api.ToolKit.Config;
using Mozu.AubuchonDataAdapter.Domain.CatalogImportService;
using Mozu.AubuchonDataAdapter.Domain.FileUploadService;
using Mozu.AubuchonDataAdapter.Domain.InventoryImportService;
using Product = Mozu.Api.Contracts.ProductAdmin.Product;


namespace Mozu.AubuchonDataAdapter.Domain.Handlers
{
    public interface IProductExportHandler
    {
        Task<Product> GetProduct(IApiContext apiContext, string productCode);
        Task<String> ExportFileuploadAsync(List<ProductLocalizedImage> images);
        Task<String> ExportInventoryAsync(IApiContext apiContext, Product product, IList<LocationInventory> locations);
        Task<String> ExportProductAsync(IApiContext apiContext, Product product);
    }

    public class ProductExportHandler : HandlerBase, IProductExportHandler
    {
        private readonly String _productImportServiceUrl;
        private readonly String _inventoryImportServiceUrl;
        private readonly String _fileUploadServiceUrl;

        public ProductExportHandler(IAppSetting appSetting) : base(appSetting)
        {
            _productImportServiceUrl = appSetting.Settings["CatalogImportService"].ToString();
            _inventoryImportServiceUrl = appSetting.Settings["InventoryImportService"].ToString();
            _fileUploadServiceUrl = appSetting.Settings["FileUploadService"].ToString();
        }

        public async Task<Product> GetProduct(IApiContext apiContext, string productCode)
        {
            if (String.IsNullOrEmpty(productCode)) return null;
            var productResource = new Api.Resources.Commerce.Catalog.Admin.ProductResource(apiContext);
            return await productResource.GetProductAsync(productCode);
        }

        public async Task<String> ExportProductAsync(IApiContext apiContext, Product product)
        {
            await product.FillProductLocalizedImages(apiContext);
            var productExportXml = product.BuildProductExportMessageAsync(ProviderCode, XRefMerchantId);
            var client = new CatalogImport_wrapService {Url = _productImportServiceUrl};
            var result = await ExportProductAsyncTask(client, productExportXml);
            return result.Result; 
        }


        public async Task<String> ExportInventoryAsync(IApiContext apiContext, Product product, IList<LocationInventory> locations)
        {
            await product.FillProductLocalizedImages(apiContext);
            var inventoryExportXml = product.BuildInventoryExportMessageAsync(ProviderCode, XRefMerchantId, locations);
            var client = new InventoryImport_wrapService{Url = _inventoryImportServiceUrl};
            var result = await ExportInventoryAsyncTask(client, inventoryExportXml);
            return result.Result;
        }

        public async Task<String> ExportFileuploadAsync(List<ProductLocalizedImage> images)
        {
            var fileUploadExportXml = images.BuildFileUploadExportMessageAsync(ProviderCode, XRefMerchantId);
            var client = new FileUpload_wrapService {Url = _fileUploadServiceUrl};
            var result = await ExportFileuploadAsyncTask(client, fileUploadExportXml);
            return result.Result;
        }

        Task<CatalogImportService.acceptCompletedEventArgs> ExportProductAsyncTask(CatalogImport_wrapService client, string exportXml)
        {
            var tcs = new TaskCompletionSource<CatalogImportService.acceptCompletedEventArgs>();
            client.acceptCompleted += (sender, e) => TransferCompletion(tcs, e, () => e);
            client.acceptAsync(exportXml, ServiceUsername, ServicePassword);
            return tcs.Task;
        }


        Task<InventoryImportService.acceptCompletedEventArgs> ExportInventoryAsyncTask(InventoryImport_wrapService client, string exportXml)
        {
            var tcs = new TaskCompletionSource<InventoryImportService.acceptCompletedEventArgs>();
            client.acceptCompleted += (sender, e) => TransferCompletion(tcs, e, () => e);
            client.acceptAsync(exportXml, ServiceUsername, ServicePassword);
            return tcs.Task;
        }

        Task<FileUploadService.acceptCompletedEventArgs> ExportFileuploadAsyncTask(FileUpload_wrapService client, string exportXml)
        {
            var tcs = new TaskCompletionSource<FileUploadService.acceptCompletedEventArgs>();
            client.acceptCompleted += (sender, e) => TransferCompletion(tcs, e, () => e);
            client.acceptAsync(exportXml, ServiceUsername, ServicePassword);
            return tcs.Task;
        }

    }
}
