using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc
{
    /// <summary>
    /// Model Base
    /// </summary>
    [ModelBinder(typeof(ASModelBinder))]
    public abstract class ASModelBase
    {
        public ViewModelHeader Header { get; private set; }
        public string CaptchaPublicKey { get; set; }
        public bool DisplayCaptcha { get; set; }

        public ASModelBase()
        {
            this.Header = new ViewModelHeader();
            this.DisplayCaptcha = false;
        }
    }
}