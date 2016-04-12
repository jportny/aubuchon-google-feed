using System;
using System.Threading.Tasks;
using Mozu.Api.ToolKit.Config;
using Mozu.AubuchonDataAdapter.Domain.Utility;

namespace Mozu.AubuchonDataAdapter.Domain.Handlers
{
    public abstract class HandlerBase
    {
        protected readonly string ProviderCode;
        protected readonly string XRefMerchantId;
        protected string ServiceUsername;
        protected string ServicePassword;


        protected IAppSetting AppSetting;
        protected HandlerBase(IAppSetting appSetting)
        {
            AppSetting = appSetting;
            var providerCode = appSetting.Settings["ProviderCode"];
            var xRefMerchantId = appSetting.Settings["XrefMerchantId"];

            if (providerCode == null)
            {
                throw new ArgumentException("Provider Code is not present in the Config!");
            }
            if (xRefMerchantId == null)
            {
                throw new ArgumentException("XRefMerchantId is not present in the Config!");
            }
            ProviderCode = providerCode.ToString();
            XRefMerchantId = xRefMerchantId.ToString();
            ServiceUsername = SecureAppSetting.Decrypt(appSetting.Settings["EdgeServiceUsername"].ToString());
            ServicePassword = SecureAppSetting.Decrypt(appSetting.Settings["EdgeServicePassword"].ToString());
        }
        protected static void TransferCompletion<T>(TaskCompletionSource<T> tcs, System.ComponentModel.AsyncCompletedEventArgs e, Func<T> getResult)
        {
            if (e.Error != null)
            {
                tcs.TrySetException(e.Error);
            }
            else if (e.Cancelled)
            {
                tcs.TrySetCanceled();
            }
            else
            {
                tcs.TrySetResult(getResult());
            }
        }
    }
}
