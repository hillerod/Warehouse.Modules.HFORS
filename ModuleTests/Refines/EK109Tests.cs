using Bygdrift.Warehouse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Module;
using System;
using System.IO;

namespace ModuleTests.Refines
{
    [TestClass]
    public class EK109Tests:BaseTests
    {
        [TestMethod]
        public void CallTest()
        {
            var fileName = "Hillerod-Kommune-EK109-20220311T022304.csv";
            var importPath = Path.Combine(BasePath, "Files", "FromFTP", fileName);
            var stream = new FileStream(importPath, FileMode.Open);
            var csv = new Module.Refines.Helpers.EK109(App, CsvConfig).ToCsv(stream, true);
            var exportPath = Path.Combine(BasePath, "Files", "FromFTP", fileName + "_Converted.csv");
            csv.ToCsvFile(importPath);
        }
    }
}
