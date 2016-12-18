using AS.Domain.Interfaces;
using FluentScheduler;
using System.Web.Hosting;

namespace AS.Jobs
{
    public abstract class JobBase : IJob, IRegisteredObject
    {
        private readonly object _lock = new object();
        private bool _shuttingDown;
        private readonly ILogger _logger;

        public JobBase(ILogger logger)
        {
            this._logger = logger;
            HostingEnvironment.RegisterObject(this);
        }

        public void Execute()
        {
            lock (_lock)
            {
                if (_shuttingDown)
                    return;
            }
            _logger.Debug(string.Format("{0} execution has started.", this.GetType()));
            OnExecute();
        }

        public void Stop(bool immediate)
        {
            // Locking here will wait for the lock in Execute to be released until this code can continue.
            lock (_lock)
            {
                _shuttingDown = true;
            }
            HostingEnvironment.UnregisterObject(this);
            _logger.Debug(string.Format("Job is stopped : {0}", this.GetType().FullName));
        }

        protected abstract void OnExecute();
    }
}