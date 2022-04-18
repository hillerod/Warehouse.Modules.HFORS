using Bygdrift.DataLakeTools;
using Bygdrift.Warehouse.Helpers.Logs;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Module.AppFunctions;
using Moq;
using System.Linq;
using System.Threading.Tasks;

namespace ModuleTests.Refines
{
    /// To run this test, then first add an Azure environmed, as decribed here: https://github.com/Bygdrift/Warehouse
    /// Then fetch the Azure App config connections tring and paste it to this project's User Secret like: {"ConnectionStrings:AppConfig": "the connectionstring to app config"}.
    [TestClass]
    public class TimerTriggerTests
    {
        private readonly Mock<ILogger<TimerTrigger>> loggerMock = new();
        private readonly TimerTrigger function;

        public TimerTriggerTests()
        {
            function = new TimerTrigger(loggerMock.Object);
        }

        [TestMethod]
        public async Task TimerTrigger()
        {
            //Clear the data in the warehouse for this module:
            await function.App.DataLake.DeleteDirectoryAsync("", FolderStructure.Path);

            //Clear the database for the two tables:
            function.App.Mssql.DeleteTable("Forbrug");

            //Run the function:
            await function.TimerTriggerAsync(default);

            //There should come no errors
            var errors = function.App.Log.GetErrorsAndCriticals().ToList();
            Assert.AreEqual(0, errors.Count);

            //There should come  a warning that 'Settings:DataNoteSet' is not set
            var warnings = function.App.Log.GetLogs(LogType.Warning).ToList();
            Assert.AreEqual(0, warnings.Count);

            var infos = function.App.Log.GetLogs(LogType.Information).ToList();
            Assert.AreEqual(4, infos.Count);

            //Is data uploaded to database?:
            var csvFromDb = function.App.Mssql.GetAsCsv("Forbrug");
            Assert.IsTrue(csvFromDb.RowCount > 0);
        }
    }
}