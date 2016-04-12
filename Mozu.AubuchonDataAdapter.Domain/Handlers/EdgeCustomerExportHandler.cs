using System;
using System.Threading.Tasks;
using Mozu.Api;
using Mozu.Api.Contracts.Customer;
using Mozu.Api.ToolKit.Config;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Mozu.AubuchonDataAdapter.Domain.CustomerImportService;
using Mozu.AubuchonDataAdapter.Domain.Mappers;
using acceptCompletedEventArgs = Mozu.AubuchonDataAdapter.Domain.CustomerImportService.acceptCompletedEventArgs;

namespace Mozu.AubuchonDataAdapter.Domain.Handlers
{
    public interface IEdgeCustomerExportHandler
    {
        string BuildCustomerExportMessageAsync(IApiContext apiContext, CustomerAccount account, StatusCode statusCode);
        Task<string> ExportCustomerAsync(IApiContext apiContext, CustomerAccount account, StatusCode statusCode);
    }

    public class EdgeCustomerExportHandler : HandlerBase, IEdgeCustomerExportHandler
    {
        private readonly String _memberImportServiceUrl;
       
        public EdgeCustomerExportHandler(IAppSetting appSetting) : base(appSetting)
        {
            _memberImportServiceUrl = appSetting.Settings["MemberImportService"].ToString();
           
        }

        public string BuildCustomerExportMessageAsync(IApiContext apiContext, CustomerAccount account, StatusCode statusCode)
        {
            return account != null ? account.ToEdgeCustomerExportMessage(ProviderCode, XRefMerchantId, statusCode).ToXml() : String.Empty;
        }

        public async Task<string> ExportCustomerAsync(IApiContext apiContext, CustomerAccount account, StatusCode statusCode)
        {
            var productExportXml = BuildCustomerExportMessageAsync(apiContext, account, statusCode);
            var client = new MemberImport_wrapService {Url = _memberImportServiceUrl};
            var result = await ExportCustomerAsyncTask(client, productExportXml);
            return result.Result;
        }


        Task<acceptCompletedEventArgs> ExportCustomerAsyncTask(MemberImport_wrapService client, string exportXml)
        {
            var tcs = new TaskCompletionSource<acceptCompletedEventArgs>();
            client.acceptCompleted += (sender, e) => TransferCompletion(tcs, e, () => e);
            client.acceptAsync(exportXml, ServiceUsername, ServicePassword);
            return tcs.Task;
        }
    }
}
