using System.Collections.Generic;

namespace AS.Infrastructure.Logging
{
    public static class LogLevels
    {
        public static List<string> All
        {
            get
            {
                return new List<string>()
                {
                "DEBUG", "INFO", "WARN", "ERROR"
                };
            }
        }
    }
}