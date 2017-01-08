namespace ASPWiki.Services.Tasks
{
    public interface IRecurringJob
    {
        string GetJobId();
        void Execute();
        string CronInterval();
    }
}
