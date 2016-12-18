using AS.Domain.Interfaces;
using AS.Domain.Settings;
using System.Net.Http;

namespace AS.Jobs
{
    /// <summary>
    /// Even this job is called "PingJob" it is not related with ICMP.
    /// It just visits a web-url. We use this job to ping our web-site to  keep it alive.
    /// If our hosting package does not allow us to change settings of IIS to
    /// By defaul application sleeps if not receives any request in a time interval
    /// We use this job to keep our application alive.
    /// </summary>
    public class PingJob : JobBase
    {
        private const int TimeOut = 5000;
        private readonly ISettingManager _settingManager;

        public PingJob(ILogger logger, ISettingManager settingManager)
            : base(logger)
        {
            this._settingManager = settingManager;
        }

        protected override void OnExecute()
        {
            if (this._settingManager.GetContainer<UrlAddress>().Contains("PingUrl"))
            {
                using (HttpClient client = new HttpClient())
                {
                    client.GetAsync(this._settingManager.GetContainer<UrlAddress>()["PingUrl"].Address)
                        .Wait(TimeOut);
                }
            }
        }
    }
}