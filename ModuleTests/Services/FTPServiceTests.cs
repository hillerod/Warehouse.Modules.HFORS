using Bygdrift.Warehouse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Module;
using System.Linq;

namespace ModuleTests.Services
{
    [TestClass]
    public class FTPServiceTests
    {
        public FTPServiceTests() => App = new AppBase<Settings>();

        public AppBase<Settings> App { get; private set; }

        [TestMethod]
        public void CallTest()
        {
            App.Log.LogInformation($"The module '{App.ModuleName}' is started");
            var ftpService = new Module.Services.FTPService(App, App.Settings.FTPConnectionMeterReadings);
            var csvFiles = ftpService.GetData().ToList();
        }
    }
}
