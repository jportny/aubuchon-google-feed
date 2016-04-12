using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Mozu.AubuchonDataAdapter.Domain.Handlers.Excel;
using System.IO;
using System.Net.Http.Headers;
using Mozu.Api.WebToolKit.Filters;
namespace Mozu.AubuchonDataAdapter.Web.Controllers
{
    public class FileController : ApiController
    {
        private readonly IImportExportLocationMZExtras _locationExtraHandler;
        public FileController(IImportExportLocationMZExtras importLocationExtra) {
            _locationExtraHandler = importLocationExtra;
        }

        [ConfigurationAuthFilter]
        [HttpPost]
        public HttpResponseMessage List(int? tenantId) {

            var dInfo = new DirectoryInfo(_locationExtraHandler.GetImportPath(tenantId));

            var files = dInfo.EnumerateFiles("*.xlsx")
                                .OrderByDescending(fi => fi.LastWriteTime)
                                    .Select(f => new { fullName = f.FullName, fileName = f.Name }).ToArray();

           return Request.CreateResponse(HttpStatusCode.OK, files);
        }

        [ConfigurationAuthFilter]
        [HttpGet]
        public HttpResponseMessage Download(int tenantId, string fileName)
        {
            var request = ControllerContext.Request;
            
            try
            {
                if (File.Exists(fileName))
                {
                    var response = request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StreamContent(File.OpenRead(fileName));
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = Path.GetFileName(fileName)
                    };

                    response.Content.Headers.Add("Content-Description", "File Transfer");
                    response.Content.Headers.Add("Content-Transfer-Encoding", "binary");
                    response.Content.Headers.Add("Content-Length", (new FileInfo(fileName)).Length.ToString());
                    return response;
                }
                return request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception exc)
            {
                //_logger.Error(exc.Message, exc);
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
            }

        }
    }
}
