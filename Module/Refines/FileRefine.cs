using Bygdrift.CsvTools;
using Bygdrift.DataLakeTools;
using Bygdrift.Warehouse;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Module.Refines
{
    public class FileRefine
    {
        public static async Task Refine(AppBase<Settings> app, (DateTime Saved, string Name, Stream stream) file)
        {
            app.Log.LogInformation("Refining data...");
            app.LoadedLocal = file.Saved;

            await app.DataLake.SaveStreamAsync(file.stream, "Raw", file.Name, FolderStructure.DatePath);

            var csv = Helpers.EK109.ToCsv(file.stream, true);
            await app.DataLake.SaveCsvAsync(csv, "Refined", $"Data_{file.Saved.ToString("yyyyMMddTHHmmss")}.csv", FolderStructure.DatePath);

            app.Mssql.MergeCsv(csv, "Data", "Id", false, false);
        }
    }
}