using AS.Infrastructure.Reflection;
using System.Web;

namespace AS.Infrastructure.Web.Reflection
{
    /// <summary>
    /// TypeFinder for web application
    /// </summary>
    public class WebTypeFinder : TypeFinder
    {
        protected override string Dir
        {
            get
            {
                return HttpRuntime.BinDirectory;
            }
        }
    }
}