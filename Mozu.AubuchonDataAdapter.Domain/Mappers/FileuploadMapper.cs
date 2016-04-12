using System;
using System.Collections.Generic;
using System.Globalization;
using Mozu.Api.Contracts.ProductAdmin;
using Mozu.AubuchonDataAdapter.Domain.Contracts;

namespace Mozu.AubuchonDataAdapter.Domain.Mappers
{
    public static class FileuploadMapper
    {
        public static FileuploadMessage ToEdgeFileuploadExportMessage(this List<ProductLocalizedImage> images, string providerCode,
            string xrefMerchantId)
        {
            var message = new FileuploadMessage();

            var head = new MessageHead
            {
                Sender = new MessageHeadSender[1],
                Recipient = new MessageHeadRecipient[1],
                Messageid = Guid.NewGuid().ToString(),
                Messagetype = "Fileupload",
                Date = DateTime.Now.ToString(CultureInfo.InvariantCulture)
            };
            var sender = new MessageHeadSender { Systemid = providerCode };
            head.Sender[0] = sender;

            var recipient = new MessageHeadRecipient { Companyid = xrefMerchantId };
            head.Recipient[0] = recipient;
            message.Items = new object[2];
            message.Items[0] = head;


            var fileupload = new MessageBodyFileupload();

            if (images != null)
            {
                fileupload.Files = new MessageBodyFileuploadFilesFile[images.Count];

                for (var i = 0; i < images.Count; i++)
                {
                    fileupload.Files[i] = new MessageBodyFileuploadFilesFile
                    {

                        Title = images[i].ImageLabel,
                        Filename = images[i].ImageLabel,
                        Fileclass = "image",
                        FileLink = String.Format("{0}{1}",
                        images[i].ImageUrl.Contains("http:")
                        ? "" : "http:", images[i].ImageUrl),
                        Locales = new[] { new MessageBodyFileuploadFilesFileLocalesLocale { Description = String.Empty, Title = String.Empty, LocaleCode = String.Empty} }
                    };

                }
            }

            var body = new FileuploadMessageBody { Fileupload = new[] { fileupload } };
            message.Items[1] = body;
            return message;
        }
    }
}
