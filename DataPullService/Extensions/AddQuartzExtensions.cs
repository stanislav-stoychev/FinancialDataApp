using DataPullService.Jobs;
using DataPullService.Options;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace DataPullService.Extensions;

public static class AddQuartzExtensions
{
    public static IServiceCollection AddQuartzJobs(
        this IServiceCollection services,
        DataPullServiceOptions options
    )
    => services.AddQuartz(cfg =>
    {
        cfg.UseMicrosoftDependencyInjectionJobFactory();

        cfg.AddJob(
            typeof(DataPullJob),
            null,
            c => c.WithIdentity(name: nameof(DataPullJob), group: "DataPull"));

        cfg.AddTrigger(
            t => t.WithIdentity(name: "DataPullJobTrigger", group: "DataPullTrigger")
            .ForJob(new JobKey(nameof(DataPullJob), "DataPull"))
            .WithCronSchedule(options.PullFinancialDataCronExpression, x => x.WithMisfireHandlingInstructionFireAndProceed())
            .WithDescription("Job for pulling financial data"));
    })
    .AddQuartzHostedService(o =>
    {
        o.WaitForJobsToComplete = true;
    });
}
