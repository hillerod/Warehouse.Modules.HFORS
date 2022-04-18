using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace ModuleTests.Refines
{
    [TestClass]
    public class EK109Tests
    {
        /// <summary>Path to project base</summary>
        public static readonly string BasePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));

        [TestMethod]
        public void CallTest()
        {
            var fileName = "Hillerod-Kommune-EK109-20220311T022304.csv";
            var importPath = Path.Combine(BasePath, "Files", "FromFTP", fileName);
            var stream = new FileStream(importPath, FileMode.Open);
            var csv = Module.Refines.Helpers.EK109.ToCsv(stream, true);
            var exportPath = Path.Combine(BasePath, "Files", "FromFTP", fileName + "_Converted.csv");
            csv.ToCsvFile(importPath);
        }
    }
}
