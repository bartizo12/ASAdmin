using AS.Domain.Interfaces;
using AS.Services.Interfaces;

namespace AS.Jobs
{
    /// <summary>
    ///  Async Job that resets demo application. Re-installs original data
    /// </summary>
    public class DemoResetJob : JobBase
    {
        private readonly IInstallerService _installerService;

        public DemoResetJob(IInstallerService installerService, ILogger logger)
            : base(logger)
        {
            this._installerService = installerService;
        }

        protected override void OnExecute()
        {
            _installerService.Install();
        }
    }
}