using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Mozu.Api.WebToolKit.Filters;
using Mozu.AubuchonDataAdapter.Domain.Handlers.Excel;

namespace Mozu.AubuchonDataAdapter.Web.Controllers
{
    
    public class ExportController : ApiController
    {
        #region Members
        private readonly IImportExportLocationMZExtras _exportLocation;
        #endregion
        
        #region Constructor
        public ExportController(IImportExportLocationMZExtras exportLocation)
        {
            _exportLocation = exportLocation;
        }
        #endregion

        [ConfigurationAuthFilter]
        [HttpPost]
        public HttpResponseMessage Default(int tenantId)
        {
            try
            {
                _exportLocation.AddExportJobs(tenantId);
               return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
               return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
