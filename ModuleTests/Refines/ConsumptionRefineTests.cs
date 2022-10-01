using Bygdrift.Tools.CsvTool;
using Bygdrift.Tools.CsvTool.TimeStacking;
using Bygdrift.Tools.CsvTool.TimeStacking.Models;
using Bygdrift.Warehouse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Module;
using System;
using System.IO;

namespace ModuleTests.Refines
{
    [TestClass]
    public class ConsumptionRefineTests : BaseTests
    {
        [TestMethod]
        public void CallTest()
        {
            var csvFromFtp = GetCsv();
            csvFromFtp.ToCsvFile(Path.Combine(BasePath, "Files", "Out", "out.csv"));
            var consumptionRefine = new Module.Refines.ConsumptionRefine(App);

            var csvPerHour = consumptionRefine.GetPartitionedCsvPerHour(csvFromFtp);
            csvPerHour.ToCsvFile(Path.Combine(BasePath, "Files", "Out", "csvPerHour.csv"));
            //Module.Refines.ConsumptionRefine.Refine(app, csvFromFtp);
        }

        private Csv GetCsv()
        {
            var fileName = "Hillerod-Kommune-EK109-20220311T022304.csv";
            var importPath = Path.Combine(BasePath, "Files", "FromFTP", fileName);
            var stream = new FileStream(importPath, FileMode.Open);
            return new Module.Refines.Helpers.EK109(App, CsvConfig).ToCsv(stream, true);
        }
    }
}
