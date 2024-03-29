using Bygdrift.Warehouse;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Module.Services;
using System;
using System.Threading.Tasks;

namespace Module.AppFunctions
{
    public class TimerTrigger
    {
        public TimerTrigger(ILogger<TimerTrigger> logger) => App = new AppBase<Settings>(logger);

        public AppBase<Settings> App { get; private set; }

        [FunctionName(nameof(TimerTriggerAsync))]
        public async Task TimerTriggerAsync([TimerTrigger("%ScheduleExpression%"
#if DEBUG
            , RunOnStartup = true
#endif
	)] TimerInfo myTimer)
        {
            App.Log.LogInformation($"The module '{App.ModuleName}' is started");

            var ftpService = new FTPService(App, App.Settings.FTPConnectionMeterReadings);
            var ftpDeliver = new FTPService(App, App.Settings.FTPConnectionDeliverToME2);
            var consumptionRefine = new Refines.ConsumptionRefine(App);

            foreach (var file in ftpService.GetData())
            {
                await consumptionRefine.Refine(file, true, true);
                ftpDeliver.AddContent(file.Name, file.stream);
            }

            //Refines.EK109ExplanationTables.Refine(App);
            ftpService.MoveFolderContent("Backup");
            ftpService.Close();
            ftpDeliver.Close();

            //ftpService = new FTPService(App, App.Settings.FTPConnectionCustomerData);
            //foreach (var file in ftpService.GetData())
            //    await Refines.CustomerDataRefine.Refine(App, file);

            //ftpService.Close();

            //var timeToKeepMeteringsPerHour = DateTime.Now.AddMonths(-App.Settings.MonthsToKeepMeteringsPerHour);
            //App.Mssql.DeleteOldRows("ForbrugPrTime", "Fra", timeToKeepMeteringsPerHour);
        }
    }
}