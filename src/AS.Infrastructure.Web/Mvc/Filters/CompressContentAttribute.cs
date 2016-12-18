using System;
using System.IO.Compression;
using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc.Filters
{
    /// <summary>
    /// Applies Deflate or Gzip compression to HttpResponse. If client supports Deflate , applies
    /// Deflate compression. If client accepts Gzip compression, applies Gzip. If client supports
    /// none , does not apply any compression
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class CompressContentAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string encodingsAccepted = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(encodingsAccepted)) return;

            encodingsAccepted = encodingsAccepted.ToUpperInvariant();
            var response = filterContext.HttpContext.Response;

            if (encodingsAccepted.Contains("DEFLATE"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
            else if (encodingsAccepted.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
        }
    }
}