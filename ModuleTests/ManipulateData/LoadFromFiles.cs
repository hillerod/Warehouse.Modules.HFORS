using Bygdrift.Tools.CsvTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Module.Refines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModuleTests
{
    [TestClass]
    public class LoadFromFiles : BaseTests
    {
        [TestMethod]
        public async Task Start()
        {
            int? take = 1;
            IEnumerable<File> files = LoadAllCsv().OrderBy(o => o.Date);
            if (take != null)
               files = files.Take((int)take);

            var refine = new ConsumptionRefine(App);
            var counter = 1;
            foreach (var item in files)
            {
                App.Log.LogInformation($"Loading {counter++} of {files.Count()}...");
                App.LoadedLocal = item.Date;
                await refine.Refine(item.GetFile(), false, true);
                Assert.IsFalse(App.Log.HasErrorsOrCriticals());
            }
        }

        private List<File> LoadAllCsv(int? take = null)
        {
            var res = new List<File>();
            var directoryPath = Path.Combine(BasePath, "Files", "In", "Raw");
            var filePaths = Directory.GetFiles(directoryPath, "*.csv", SearchOption.AllDirectories);

            for (int i = 0; i < filePaths.Length; i++)
            {
                if (take != null && i == take)
                    break;

                var name = Path.GetFileName(filePaths[i]);
                if (name != "device_info.csv")
                {
                    var saved = DateTime.ParseExact(name.Substring(23, 15), "yyyyMMdd'T'HHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None);
                    res.Add(new File(name, filePaths[i], saved));
                }
            }

            return res;
        }
    }

    public class File
    {
        public File(string name, string path, DateTime date)
        {
            Name = name;
            Path = path;
            Date = date;
        }

        public string Name { get; set; }

        public string Path { get; set; }

        public DateTime Date { get; set; }

        public (DateTime Saved, string Name, Stream stream) GetFile()
        {
            var stream = new FileStream(Path, FileMode.Open);
            return (Date, Name, stream);
        }
    }
}
