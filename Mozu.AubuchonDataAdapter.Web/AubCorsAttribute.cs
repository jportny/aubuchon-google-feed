using System;
using System.Linq;
using System.Web.Http.Cors;
using System.Web.Cors;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mozu.AubuchonDataAdapter.Web
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AubCorsAttribute : Attribute, ICorsPolicyProvider
    {
        private readonly CorsPolicy _policy;

        public AubCorsAttribute(string methods, string headers)
        {

            _policy = new CorsPolicy();
            
            MvcApplication.AllowedOrigins.Split(',').ToList().ForEach(origin => _policy.Origins.Add(origin.Trim()));
            
            if (methods.Trim().Equals("*"))
                _policy.AllowAnyMethod = true;
            else
                methods.Split(',').ToList().ForEach(method => _policy.Methods.Add(method.Trim()));

            if (headers.Trim().Equals("*"))
                _policy.AllowAnyHeader = true;
            else
                headers.Split(',').ToList().ForEach(header => _policy.Headers.Add(header.Trim()));

        }
        public System.Threading.Tasks.Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult<CorsPolicy>(_policy);
        }
    }
}