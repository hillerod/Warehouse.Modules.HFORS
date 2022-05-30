using Bygdrift.CsvTools;
using Bygdrift.DataLakeTools;
using Bygdrift.Warehouse;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Module.Refines
{
    public class CustomerDataRefine
    {
        public static async Task Refine(AppBase<Settings> app, (DateTime Saved, string Name, Stream stream) file)
        {
            app.Log.LogInformation("Refining customer data...");
            app.LoadedLocal = file.Saved;

            await app.DataLake.SaveStreamAsync(file.stream, "Raw", file.Name, FolderStructure.DatePath);
            var csv = new Csv().FromCsvStream(file.stream, ';').AddColumn("Indlæst", app.LoadedLocal, false);

            app.Mssql.MergeCsv(csv, "Målere", "device.serialNo", true, false);
        }
    }
}