using ASPWiki.Services.Tasks;
using Hangfire;
using System;
using System.Collections.Generic;

namespace ASPWiki.Services
{
    public class TaskScheluder
    {
        private Dictionary<string, IRecurringJob> recurringJobs;

        public TaskScheluder()
        {
            recurringJobs = new Dictionary<string, IRecurringJob>();
        }

        public void AddJob(IRecurringJob job, string jobID = null)
        {
            if (String.IsNullOrEmpty(jobID))
            {
                jobID = job.GetJobId();
            }

            recurringJobs.Add(jobID, job);
            RecurringJob.AddOrUpdate(jobID, () => job.Execute(), job.CronInterval);
        }

        public void UpdateJob(string jobID)
        {
            if (recurringJobs.ContainsKey(jobID))
            {
                RecurringJob.AddOrUpdate(jobID, () => recurringJobs[jobID].Execute(), recurringJobs[jobID].CronInterval);
            }
            else
            {
                throw new Exception("Job: " + jobID + " does not exists");
            }
        }

        public void RemoveJob(string jobID)
        {
            RecurringJob.RemoveIfExists(jobID);
            recurringJobs.Remove(jobID);
        }
    }
}
