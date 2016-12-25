namespace AS.Domain.Entities
{
    /// <summary>
    /// Scheduled Job/Task Status
    /// </summary>
    public enum JobStatus
    {
        Queued = 0,
        Running = 1,
        Finished = 2,
        Failed = 3
    }
}