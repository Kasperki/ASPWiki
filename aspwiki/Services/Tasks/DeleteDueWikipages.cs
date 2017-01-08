using Hangfire;
using System;

namespace ASPWiki.Services.Tasks
{
    public class DeleteDueWikipages : IRecurringJob
    {
        public const string JOB_ID = "DELETE_DUE_WIKIPAGES";

        private readonly IWikiRepository wikiRepo;

        public DeleteDueWikipages(IWikiRepository wikiRepo)
        {
            this.wikiRepo = wikiRepo;
        }

        public string CronInterval()
        {
            return Cron.MinuteInterval(5);
        }

        public void Execute()
        {
            var wikipages = wikiRepo.GetAllPrivate();

            foreach (var wiki in wikipages)
            {
                if (wiki.DueDate != null && Convert.ToDateTime(wiki.DueDate) < DateTime.Now)
                {
                    wikiRepo.Delete(wiki, false);
                }
            }
        }

        public string GetJobId()
        {
            return JOB_ID;
        }
    }
}
