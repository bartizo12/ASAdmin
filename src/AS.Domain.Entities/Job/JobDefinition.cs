using System;

namespace AS.Domain.Entities
{
    /// <summary>
    /// Represents definition of a scheduled job which works asynchronously
    /// </summary>
    [Serializable]
    public class JobDefinition : TrackableEntityBase<int>
    {
        public string Name { get; set; }
        public string JobTypeName { get; set; }
        public string Error { get; set; }
        public string Comment { get; set; }
        public int RunInterval { get; set; } //In Seconds
        public JobStatus JobStatus { get; set; }
        public DateTime? LastExecutionTime { get; set; }
    }
}