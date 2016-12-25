using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure;
using AS.Infrastructure.Data;
using AS.Jobs;
using AS.Services.Interfaces;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace AS.Services
{
    /// <summary>
    /// Schedules and stores asynchronous jobs/tasks.
    /// </summary>
    public class SchedulerService : ISchedulerService
    {
        private readonly Func<DbConnectionConfiguration> _dbConnectionConfigurationFactory;
        private readonly IDbContext _dbContext;
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ILogger _logger;
        private readonly ITypeFinder _typeFinder;

        public SchedulerService(IDbContext dbContext,
            Func<DbConnectionConfiguration> dbConnectionConfigurationFactory,
            IDbContextFactory dbContextFactory,
            ITypeFinder typeFinder,
            ILogger logger)
        {
            this._dbContext = dbContext;
            this._dbContextFactory = dbContextFactory;
            this._typeFinder = typeFinder;
            this._logger = logger;
            this._dbConnectionConfigurationFactory = dbConnectionConfigurationFactory;
        }

        public void CreateJobDefinition(JobDefinition jobDefinition)
        {
            _dbContext.Set<JobDefinition>().Add(jobDefinition);
            _dbContext.SaveChanges();
            ScheduleJob(jobDefinition);
        }

        public void UpdateJobDefinition(JobDefinition jobDefinition)
        {
            _dbContext.Entry(jobDefinition).State = EntityState.Modified;
            _dbContext.SaveChanges();
            ScheduleJob(jobDefinition);
        }

        /// <summary>
        /// Get JobDefinition by ID
        /// </summary>
        /// <param name="id">Job/Task ID</param>
        /// <returns>Found job definition or null</returns>
        public JobDefinition GetById(int id)
        {
            return _dbContext.Set<JobDefinition>().SingleOrDefault(jobDef => jobDef.Id == id);
        }

        /// <summary>
        /// Get JobDefinition by name
        /// </summary>
        /// <param name="name">Name of the job</param>
        /// <returns>Found job definition or null</returns>
        public JobDefinition GetByName(string name)
        {
            return _dbContext.Set<JobDefinition>().AsNoTracking().SingleOrDefault(jobDef => jobDef.Name == name);
        }

        /// <summary>
        /// Fetches/selects all job definitions. Since we know there will not be many jobs we will not apply
        /// filtering or pagination.
        /// </summary>
        /// <param name="ordering">OrderBy clause</param>
        /// <returns>List of job definitions</returns>
        public IList<JobDefinition> FetchJobDefinitions(string ordering)
        {
            using (IDbContext dbContext = _dbContextFactory.Create())
            {
                return dbContext.Set<JobDefinition>()
                                .OrderBy(ordering)
                                .ToList();
            }
        }

        /// <summary>
        /// Scans all assemblies and returns the list of full type name  of job class that implements IJob.
        /// </summary>
        /// <returns>List of Job Class Full Type Name</returns>
        public List<string> FindJobTypes()
        {
            List<string> jobTypeList = new List<string>();

            foreach (Type type in _typeFinder.FindClassesOfType<JobBase>())
            {
                jobTypeList.Add(type.AssemblyQualifiedName);
            }
            return jobTypeList;
        }

        /// <summary>
        /// Initializes the task/job scheduler
        /// </summary>
        public void Initialize()
        {
            if (_dbConnectionConfigurationFactory() == null)
                return;

            JobManager.JobException -= JobManager_JobException;
            JobManager.JobEnd -= JobManager_JobEnd;
            JobManager.JobStart -= JobManager_JobStart;

            JobManager.JobFactory = new JobFactory();
            JobManager.JobException += JobManager_JobException;
            JobManager.JobEnd += JobManager_JobEnd;
            JobManager.JobStart += JobManager_JobStart;

            using (IDbContext dbContext = _dbContextFactory.Create())
            {
                foreach (JobDefinition jobDefinition in dbContext.Set<JobDefinition>())
                {
                    this.ScheduleJob(jobDefinition);
                }
            }
            JobManager.Start();
        }

        /// <summary>
        /// Runs a job/task immediately. In case user wants a job to be executed immediately, this function will
        /// help us
        /// </summary>
        /// <param name="name">Name of the job to be executed/run</param>
        public void Run(string name)
        {
            JobDefinition jobDef = this.GetByName(name);
            IJob job = ServiceLocator.Current.Resolve(Type.GetType(jobDef.JobTypeName)) as IJob;
            JobManager.AddJob(job,
                s => s.WithName(name)
                .ToRunNow());
        }

        /// <summary>
        /// Stops all async jobs/tasks
        /// </summary>
        public void Stop()
        {
            foreach (var schedule in JobManager.RunningSchedules)
            {
                JobManager.RemoveJob(schedule.Name);
            }
            JobManager.Stop();
        }

        #region Private

        private void ScheduleJob(JobDefinition jobDefinition)
        {
            IJob job = ServiceLocator.Current.Resolve(Type.GetType(jobDefinition.JobTypeName)) as IJob;
            JobManager.RemoveJob(jobDefinition.Name);
            JobManager.AddJob(job,
                s => s.WithName(jobDefinition.Name)
                      .ToRunEvery(jobDefinition.RunInterval)
                      .Seconds());
        }

        private void JobManager_JobException(JobExceptionInfo obj)
        {
            using (IDbContext dbContext = _dbContextFactory.Create())
            {
                this._logger.Error(obj.Exception);
                JobDefinition jobDefinition = dbContext.Set<JobDefinition>().SingleOrDefault(jobDef => jobDef.Name == obj.Name);
                jobDefinition.JobStatus = JobStatus.Failed;
                jobDefinition.Error = obj.Exception.ToString();
                dbContext.Entry(jobDefinition).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        private void JobManager_JobStart(JobStartInfo obj)
        {
            if (JobManager.GetSchedule(obj.Name) == null)
                return;
            using (IDbContext dbContext = _dbContextFactory.Create())
            {
                JobDefinition jobDefinition = dbContext.Set<JobDefinition>().SingleOrDefault(jobDef => jobDef.Name == obj.Name);

                if (jobDefinition != null)
                {
                    jobDefinition.JobStatus = JobStatus.Running;
                    dbContext.Entry(jobDefinition).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
        }

        private void JobManager_JobEnd(JobEndInfo obj)
        {
            if (JobManager.GetSchedule(obj.Name) == null)
                return;

            using (IDbContext dbContext = _dbContextFactory.Create())
            {
                JobDefinition jobDefinition = dbContext.Set<JobDefinition>().SingleOrDefault(jobDef => jobDef.Name == obj.Name);
                jobDefinition.LastExecutionTime = DateTime.UtcNow;
                jobDefinition.JobStatus = JobStatus.Finished;
                dbContext.Entry(jobDefinition).State = EntityState.Modified;
                dbContext.SaveChanges();

                this._logger.Debug(string.Format("Execution of job {0} took {1} msecs.",
                    obj.Name, (long)obj.Duration.TotalMilliseconds));
            }
        }

        #endregion Private
    }
}