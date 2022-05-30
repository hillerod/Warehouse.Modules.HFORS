using Bygdrift.Warehouse;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Module.Services;
using System.Threading.Tasks;

namespace Module.AppFunctions
{
    public class TimerTrigger
    {
        public TimerTrigger(ILogger<TimerTrigger> logger) => App = new AppBase<Settings>(logger);

        public AppBase<Settings> App { get; private set; }

        [FunctionName(nameof(TimerTriggerAsync))]
        public async Task TimerTriggerAsync([TimerTrigger("%ScheduleExpression%", RunOnStartup = true)] TimerInfo myTimer)
        {
            App.Log.LogInformation($"The module '{App.ModuleName}' is started");

            var ftpService = new FTPService(App, App.Settings.FTPConnectionMeterReadings);
            foreach (var file in ftpService.GetData())
                await Refines.ConsumptionRefine.Refine(App, file);

            Refines.EK109ExplanationTables.Refine(App);
            ftpService.MoveFolderContent("Backup");
            ftpService.Close();

            ftpService = new FTPService(App, App.Settings.FTPConnectionCustomerData);
            foreach (var file in ftpService.GetData())
                await Refines.CustomerDataRefine.Refine(App, file);

            ftpService.Close();
        }
    }
}