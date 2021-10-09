using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;

namespace Licenser.Sdk.Client
{
    /// <summary>
    /// Static class of Background scheduler.
    /// </summary>
    public static class BackgroundScheduler
    {
        /// <summary>
        /// Schedules specified background action, and background lifetime is until end of application lifetime.
        /// </summary>
        /// <param name="backgroundAction">Action to perform in background.</param>
        /// <param name="startAfterHours">Start specified action after indicated hours.</param>
        /// <param name="repeatDurationHours">Duration of hours which re-executes specified action.</param>
        /// <returns></returns>
        public static async Task StartActionAtSpecifiedTimeAsync(Action backgroundAction, 
            int startAfterHours = 12, 
            int repeatDurationHours = 12)
        {
            //LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();
            await scheduler.Start();

            var job = ActionJob(backgroundAction)
                .WithIdentity("job1", "group1")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(DateTimeOffset.Now.AddHours(startAfterHours))
                .WithSimpleSchedule(i => i
                    .WithIntervalInHours(repeatDurationHours)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        private static JobBuilder ActionJob(Action action)
        {
            return JobBuilder
                .Create<ActionJob>()
                .SetJobData(new JobDataMap
                {
                    {"action", action}
                });
        }
    }

    internal class ConsoleLogProvider : ILogProvider
    {
        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) =>
            {
                if (level >= LogLevel.Info && func != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                }
                return true;
            };
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            throw new NotImplementedException();
        }
    }

    internal class ActionJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var action = context.MergedJobDataMap["action"] as Action;
            action?.Invoke();
            return Task.CompletedTask;
        }
    }
}