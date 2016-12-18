using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc
{
    /// <summary>
    /// BaseViewPage
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract partial class ASViewPageBase<TModel> : WebViewPage<TModel>
    {
        public ASViewPageBase()
        {
        }
    }

    /// <summary>
    /// BaseViewPage
    /// </summary>
    public abstract class ASViewPageBase : WebViewPage<dynamic>
    {
        public ASViewPageBase()
        {
        }
    }
}