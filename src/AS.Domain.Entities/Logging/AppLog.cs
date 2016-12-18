using System;

namespace AS.Domain.Entities
{
    /// <summary>
    /// Application log to be used for multi purposes.
    /// Can be used for any logger API ( NLog/Log4Net/or another logger API ...etc)
    /// </summary>
    [Serializable]
    public partial class AppLog : EntityBase<long>, ISafeToDeleteEntity
    {
        /// <summary>
        /// Level of the log. DEBUG,INFO,WARN,ERROR ...etc
        /// </summary>
        public string Level { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Where it is logged in the code
        /// </summary>
        public string Location { get; set; }

        public string MachineName { get; set; }
        public string AppDomain { get; set; }
        public string LoggerName { get; set; }
        public string ClientIP { get; set; }
    }
}