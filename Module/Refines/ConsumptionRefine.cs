using Bygdrift.Tools.CsvTool;
using Bygdrift.Tools.CsvTool.TimeStacking;
using Bygdrift.Tools.CsvTool.TimeStacking.Models;
using Bygdrift.Tools.DataLakeTool;
using Bygdrift.Warehouse;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Module.Refines
{
    public class ConsumptionRefine
    {
        private readonly AppBase<Settings> app;
        private readonly Config Csvconfig;
       
        public ConsumptionRefine(AppBase<Settings> app)
        {
            this.app = app;
            Csvconfig = new Config(this.app.CultureInfo, this.app.TimeZoneInfo, FormatKind.TimeOffset);
        }

        public async Task Refine((DateTime Saved, string Name, Stream stream) file, bool saveToDataLake, bool saveToDatabase)
        {
            app.Log.LogInformation("Refining data...");
            app.LoadedLocal = file.Saved;
            await ToDataLake(file, "Raw", saveToDataLake);

            var csvForbrug = new Helpers.EK109(app, Csvconfig).ToCsv(file.stream, true);
            //csvForbrug = csvForbrug.FilterRows("Målernummer", true, 65674941);

            await ToDataLake(csvForbrug, "Refined", $"Forbrug_{file.Saved:yyyyMMddTHHmmss}.csv", saveToDataLake);
            if (csvForbrug.Records.Count == 0)
                return;

            ToDb(csvForbrug, "Forbrug", saveToDatabase);

            var csvForbrugPrTime = GetPartitionedCsvPerHour(csvForbrug);
            ToDb(csvForbrugPrTime, "ForbrugPrTime", saveToDatabase);
            //csvForbrugPrTime.ToCsvFile("c:\\Users\\kenbo\\Downloads\\ff.csv");
            await ToDataLake(csvForbrugPrTime, "Refined", $"ForbrugPrTime_{file.Saved:yyyyMMddTHHmmss}.csv", saveToDataLake);

            (DateTime From, DateTime To) = GetTimespanFromCsvPerHour(csvForbrugPrTime);
            var csvForbrugPrDag = GetPartitionedCsvPerDay(From, To);
            ToDb(csvForbrugPrDag, "ForbrugPrDag", saveToDatabase);
            await ToDataLake(csvForbrugPrDag, "Refined", $"ForbrugPrDag_{file.Saved:yyyyMMddTHHmmss}.csv", saveToDataLake);

            var csvForbrugPrMåned = GetPartitionedCsvPerMonth(From, To);
            ToDb(csvForbrugPrMåned, "ForbrugPrMåned", saveToDatabase);
            await ToDataLake(csvForbrugPrMåned, "Refined", $"ForbrugPrMåned_{file.Saved:yyyyMMddTHHmmss}.csv", saveToDataLake);
        }

        public Csv GetPartitionedCsvPerHour(Csv csv)
        {
            var res = new Csv(Csvconfig);
            var sql = $"SELECT * FROM (SELECT *,ROW_NUMBER() OVER (PARTITION BY Målernummer ORDER BY Aflæst DESC) AS [ROW NUMBER] FROM [{app.ModuleName}].[Forbrug]) groups WHERE groups.[ROW NUMBER] = 1 ORDER BY groups.Aflæst DESC";  //Inspiration: https://www.tutorialgateway.org/retrieve-last-record-for-each-group-in-sql-server/
            var LastRowInEachGroup = app.Mssql.GetAsCsvQuery(sql);
            var csvMerged = csv.FromCsv(LastRowInEachGroup, false);
            var meteringIdCols = csv.GetColRecords<string>("Målernummer").Select(o => o.Value).Distinct();
            var timeStack = CreateTimeStack(csvMerged, "Aflæst");
            var spans = timeStack.GetSpansPerHour();
            res = res.FromCsv(timeStack.GetTimeStackedCsv(spans), false);

            return res;
        }

        public Csv GetPartitionedCsvPerDay(DateTime from, DateTime to)
        {
            var res = new Csv(Csvconfig);
            var fromDay = from.Date;
            var toDay = to.Date.AddDays(1);
            var sql = $"SELECT * FROM [{app.ModuleName}].[ForbrugPrTime] where Fra >= '{fromDay:s}' AND Til <= '{toDay:s}'";
            var csvPerHour = app.Mssql.GetAsCsvQuery(sql);
            if (csvPerHour == null)
                return null;

            var meteringIdCols = csvPerHour.GetColRecords<string>("Målernummer").Select(o => o.Value).Distinct();
            var timeStack = CreateTimeStack(csvPerHour, "Fra", "Til");
            var spansPerDay = timeStack.GetSpansPerDay();
            res = res.FromCsv(timeStack.GetTimeStackedCsv(spansPerDay), false);
            return res;
        }

        public Csv GetPartitionedCsvPerMonth(DateTime from, DateTime to)
        {
            var res = new Csv(Csvconfig);
            var fromMonth = new DateTime(from.Year, from.Month, 1);
            var toMonth = new DateTime(to.Year, to.AddMonths(1).Month, 1);
            var sql = $"SELECT * FROM [{app.ModuleName}].[ForbrugPrDag] where Fra >= '{fromMonth:s}' AND Til <= '{toMonth:s}'";
            var csvPerDay = app.Mssql.GetAsCsvQuery(sql);

            var meteringIdCols = csvPerDay.GetColRecords<string>("Målernummer").Select(o => o.Value).Distinct();
            var timeStack = CreateTimeStack(csvPerDay, "Fra", "Til");
            var spansMonth = timeStack.GetSpansPerMonth();
            res = res.FromCsv(timeStack.GetTimeStackedCsv(spansMonth), false);
            return res;
        }

        /// <exception cref="ArgumentException">There has to be records in csvPerHour so it should be cheked before calling this method.</exception>
        public (DateTime From, DateTime To) GetTimespanFromCsvPerHour(Csv csvPerHour)
        {
            try
            {
                var from = csvPerHour.GetColRecords<DateTime>("Fra", false).Values.Min();
                var to = csvPerHour.GetColRecords<DateTime>("Til", false).Values.Max();
                return (from, to);
            }
            catch (Exception)
            {
                throw new ArgumentException("An error has occured. A programmer has to lokk at it.");
            }
        }

        private static TimeStack CreateTimeStack(Csv csvMerged, string headerNameFrom, string headerNameTo = null)
        {
            var timeStack = !string.IsNullOrEmpty(headerNameTo) ? new TimeStack(csvMerged, "Målernummer", headerNameFrom, headerNameTo) : new TimeStack(csvMerged, "Målernummer", headerNameFrom);
            timeStack.AddInfoFormat("Id", "[:Group]-[:From:yyyyMMddTHH]")
                    //.AddInfoFormat("Målernummer", "[:Group]")
                    .AddCalcFirstNotNull("Målernummer")
                    .AddInfoFrom("Fra")
                    .AddInfoTo("Til")
                    .AddCalcSum("Energi_Værdi", null, true)
                    .AddCalcFirstNotNull("Energi_Enhed")
                    .AddCalcFirstNotNull("Energi_Type")
                    .AddCalcSum("Volumen_Værdi", null, true)
                    .AddCalcFirstNotNull("Volumen_Enhed")
                    .AddCalcFirstNotNull("Volumen_Type")
                    //.AddCalcAny("Timer_Værdi")
                    //.AddCalcAny("Timer_Enhed")
                    //.AddCalcAny("Timer_Type")
                    .AddCalcAverage("Fremløb_Værdi")
                    .AddCalcFirstNotNull("Fremløb_Enhed")
                    .AddCalcFirstNotNull("Fremløb_Type")
                    .AddCalcAverage("Returløb_Værdi")
                    .AddCalcFirstNotNull("Returløb_Enhed")
                    .AddCalcFirstNotNull("Returløb_Type")
                    .AddCalcFirstNotNull("Energiartskode");
            return timeStack;
        }

        private void ToDb(Csv csvForbrugPrTime, string table, bool saveToDb)
        {
            if (saveToDb)
                app.Mssql.MergeCsv(csvForbrugPrTime, table, "Id", false, true);
        }

        private async Task ToDataLake((DateTime Saved, string Name, Stream stream) file, string path, bool saveToDataLake)
        {
            if (saveToDataLake)
                await app.DataLake.SaveStreamAsync(file.stream, path, file.Name, FolderStructure.DatePath);
        }

        private async Task ToDataLake(Csv csv, string path, string fileName, bool saveToDataLake)
        {
            if (saveToDataLake)
                await app.DataLake.SaveCsvAsync(csv, path, fileName, FolderStructure.DatePath);
        }
    }
}