using Mozu.Api.WebToolKit.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using Mozu.AubuchonDataAdapter.Domain.Handlers.Excel;
using Mozu.Integration.Scheduler;

namespace Mozu.AubuchonDataAdapter.Web.Controllers
{
    //[ApiAuthFilter]
    public class ImportController : ApiController
    {
        #region Members
        private readonly IImportExportLocationMZExtras _importLocation;
        #endregion
        
        #region Constructor
        public ImportController(IImportExportLocationMZExtras importLocation) 
        {
            _importLocation = importLocation;           
        }
        #endregion

        [ConfigurationAuthFilter]
        [HttpPost]
        public async Task<HttpResponseMessage> Upload(int tenantId) {
            try
            {
                var request = HttpContext.Current.Request;
                var fileContent = request.Files[0];



                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    // get a stream
                    var stream = fileContent.InputStream;
                    
                    // and optionally write the file to disk
                    var fileName = Path.GetFileName(fileContent.FileName);
                    var path = Path.Combine(_importLocation.GetImportPath(tenantId), "Import"+fileName);

                    using (var fileStream = File.Create(path))
                    {
                        stream.CopyTo(fileStream);
                    }
                   await _importLocation.AddImportJobs(tenantId, path);

                    return Request.CreateResponse(HttpStatusCode.OK, "Complete!");
                }
                else
                    throw new Exception("File not found");
               
            }
            catch (Exception exception)
            {
               return Request.CreateResponse(HttpStatusCode.BadRequest, exception.Message);
                
            }
        }
    }
}
