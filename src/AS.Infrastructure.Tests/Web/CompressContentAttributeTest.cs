using AS.Infrastructure.Web.Mvc.Filters;
using Moq;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Web;
using System.Web.Mvc;
using Xunit;

namespace AS.Infrastructure.Tests.Web
{
    public class CompressContentAttributeTest
    {
        [Theory]
        [InlineData("DEFLATE")]
        [InlineData("GZIP")]
        [InlineData("")]
        public void Compress_Should_Compress_Correctly(string encodingType)
        {
            CompressContentAttribute attribute = new CompressContentAttribute();
            Mock<ActionExecutingContext> actionContext = new Mock<ActionExecutingContext>();
            Mock<HttpContextBase> httpContext = new Mock<HttpContextBase>();
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            Mock<HttpResponseBase> response = new Mock<HttpResponseBase>();
            NameValueCollection headers = new NameValueCollection();
            string contentEncoding = string.Empty;
            Stream stream = new MemoryStream();

            headers.Add("Accept-Encoding", encodingType);
            response.Setup(r => r.AppendHeader(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((name, value) =>
                {
                    contentEncoding = value;
                });
            response.SetupGet(r => r.Filter).Returns(stream);
            response.SetupSet(r => r.Filter = It.IsAny<Stream>()).
                Callback<Stream>((_stream) =>
               {
                   stream = _stream;
               });
            request.SetupGet(r => r.Headers).Returns(headers);
            httpContext.SetupGet(h => h.Request).Returns(request.Object);
            httpContext.SetupGet(h => h.Response).Returns(response.Object);
            actionContext.SetupGet(a => a.HttpContext).Returns(httpContext.Object);

            attribute.OnActionExecuting(actionContext.Object);

            Assert.Equal(contentEncoding, encodingType.ToLowerInvariant());

            if (encodingType == "DEFLATE")
            {
                Assert.IsType(typeof(DeflateStream), stream);
            }
            else if (encodingType == "GZIP")
            {
                Assert.IsType(typeof(GZipStream), stream);
            }
            else
            {
                Assert.IsType(typeof(MemoryStream), stream);
            }
        }
    }
}