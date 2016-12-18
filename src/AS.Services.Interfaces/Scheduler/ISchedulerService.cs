using AS.Domain.Entities;
using System.Collections.Generic;

namespace AS.Services.Interfaces
{
    /// <summary>
    /// Interface that deals with scheduled asynchronous jobs/tasks
    /// </summary>
    public interface ISchedulerService : IService
    {
        /// <summary>
        /// Create asynchronous job definition
        /// </summary>
        /// <param name="jobDefinition">Job</param>
        void CreateJobDefinition(JobDefinition jobDefinition);

        /// <summary>
        /// Update asynchronous job definition
        /// </summary>
        /// <param name="jobDefinition">Job/Task</param>
        void UpdateJobDefinition(JobDefinition jobDefinition);

        /// <summary>
        /// Get JobDefinition by ID
        /// </summary>
        /// <param name="id">Job/Task ID</param>
        /// <returns>Found job definition or null</returns>
        JobDefinition GetById(int id);

        /// <summary>
        /// Get JobDefinition by name
        /// </summary>
        /// <param name="name">Name of the job</param>
        /// <returns>Found job definition or null</returns>
        JobDefinition GetByName(string name);

        /// <summary>
        /// Fetches/selects all job definitions. Since we know there will not be many jobs we will not apply
        /// filtering or pagination.
        /// </summary>
        /// <param name="ordering">OrderBy clause</param>
        /// <returns>List of job definitions</returns>
        IList<JobDefinition> FetchJobDefinitions(string ordering);

        /// <summary>
        /// Scans all assemblies and returns the list of full type name  of job class that implements IJob.
        /// </summary>
        /// <returns>List of Job Class Full Type Name</returns>
        List<string> FindJobTypes();

        /// <summary>
        /// Initializes the task/job scheduler
        /// </summary>
        void Initialize();

        /// <summary>
        /// Stops all async jobs/tasks
        /// </summary>
        void Stop();

        /// <summary>
        /// Runs a job/task immediately. In case user wants a job to be executed immediately, this function will
        /// help us
        /// </summary>
        /// <param name="name">Name of the job to be executed/run</param>
        void Run(string name);
    }
}