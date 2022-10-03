using Bygdrift.Tools.CsvTool;
using Bygdrift.Tools.CsvTool.TimeStacking;
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

        public ConsumptionRefine(AppBase<Settings> app)
        {
            this.app = app;
            app.CsvConfig.FormatKind = FormatKind.TimeOffsetDST;
        }

        public async Task Refine((DateTime Saved, string Name, Stream stream) file, bool saveToDataLake, bool saveToDatabase)
        {
            app.Log.LogInformation("Refining data...");
            app.LoadedLocal = file.Saved;
            await ToDataLake(file, "Raw", saveToDataLake);

            var csvForbrug = new Helpers.EK109(app.CsvConfig).ToCsv(file.stream, true);
            ToDb(csvForbrug, "Forbrug", saveToDatabase);
            await ToDataLake(csvForbrug, "Refined", $"Forbrug_{file.Saved:yyyyMMddTHHmmss}.csv", saveToDataLake);

            if (csvForbrug.Records.Count == 0)
                return;

            var csvForbrugPrTime = GetPartitionedCsvPerHour(csvForbrug);
            ToDb(csvForbrugPrTime, "ForbrugPrTime", saveToDatabase);
            await ToDataLake(csvForbrugPrTime, "Refined", $"ForbrugPrTime_{file.Saved:yyyyMMddTHHmmss}.csv", saveToDataLake);

            (DateTime From, DateTime To) = GetTimespanFromCsvPerHour(csvForbrugPrTime);
            var csvForbrugPrDag = GetPartitionedCsvPerDay(From, To);
            ToDb(csvForbrugPrDag, "ForbrugPrDag", saveToDatabase);
            await ToDataLake(csvForbrugPrDag, "Refined", $"ForbrugPrDag_{file.Saved:yyyyMMddTHHmmss}.csv", saveToDataLake);

            var csvForbrugPrMåned = GetPartitionedCsvPerMonth(From, To);
            ToDb(csvForbrugPrMåned, "ForbrugPrMåned", saveToDatabase);
            await ToDataLake(csvForbrugPrMåned, "Refined", $"ForbrugPrMåned_{file.Saved:yyyyMMddTHHmmss}.csv", saveToDataLake);

            var ImporteredeFilerCsv = GetFileLog(file, csvForbrug);
            ToDb(ImporteredeFilerCsv, "FilerImporteret", saveToDatabase, "Fil");
        }

        public Csv GetPartitionedCsvPerHour(Csv csv)
        {
            var lastImport = app.Mssql.GetFirstAndLastAscending<DateTimeOffset>("FilerImporteret", "AflæstMin", false).Last;
            var whereClause = "WHERE groups.[ROW NUMBER] = 1 " + (lastImport > new DateTimeOffset(2000, 1, 1, 0,0,0, new TimeSpan()) ? $"AND Aflæst >= '{app.CsvConfig.DateHelper.DateTimeToString(lastImport)}'" : "");

            var sql = $"SELECT * FROM (SELECT *,ROW_NUMBER() OVER (PARTITION BY Målernummer ORDER BY Aflæst DESC) AS [ROW NUMBER] FROM [{app.ModuleName}].[Forbrug]) groups {whereClause} ORDER BY groups.Aflæst DESC";  //Inspiration: https://www.tutorialgateway.org/retrieve-last-record-for-each-group-in-sql-server/
            var LastRowInEachGroupCsv = app.Mssql.GetAsCsvQuery(sql);

            var csvMerged = csv.FromCsv(LastRowInEachGroupCsv, false);
            var timeStack = CreateTimeStack(csvMerged, "Aflæst");
            var spansPerHour = timeStack.GetSpansPerHour();
            return timeStack.GetTimeStackedCsv(spansPerHour, app.CsvConfig);
        }

        public Csv GetPartitionedCsvPerDay(DateTime from, DateTime to)
        {
            var fromDay = from.Date;
            var toDay = to.Date.AddDays(1);
            var sql = $"SELECT * FROM [{app.ModuleName}].[ForbrugPrTime] where Fra >= '{fromDay:s}' AND Til <= '{toDay:s}'";
            var csvPerHour = app.Mssql.GetAsCsvQuery(sql);
            if (csvPerHour == null)
                return null;

            var meteringIdCols = csvPerHour.GetColRecords<string>("Målernummer").Select(o => o.Value).Distinct();
            var timeStack = CreateTimeStack(csvPerHour, "Fra", "Til");
            var spansPerDay = timeStack.GetSpansPerDay();
            return timeStack.GetTimeStackedCsv(spansPerDay, app.CsvConfig);
        }

        public Csv GetPartitionedCsvPerMonth(DateTime from, DateTime to)
        {
            var fromMonth = new DateTime(from.Year, from.Month, 1);
            var toMonth = new DateTime(to.Year, to.AddMonths(1).Month, 1);
            var sql = $"SELECT * FROM [{app.ModuleName}].[ForbrugPrDag] where Fra >= '{fromMonth:s}' AND Til <= '{toMonth:s}'";
            var csvPerDay = app.Mssql.GetAsCsvQuery(sql);

            var meteringIdCols = csvPerDay.GetColRecords<string>("Målernummer").Select(o => o.Value).Distinct();
            var timeStack = CreateTimeStack(csvPerDay, "Fra", "Til");
            var spansPerMonth = timeStack.GetSpansPerMonth();
            return timeStack.GetTimeStackedCsv(spansPerMonth, app.CsvConfig);
        }

        private Csv GetFileLog((DateTime Saved, string Name, Stream stream) file, Csv csvForbrug)
        {
            var aflæstCol = csvForbrug.GetColRecords<DateTimeOffset>("Aflæst", false);
            var aflæstMin = aflæstCol.Min(o => o.Value);
            var aflæstMax = aflæstCol.Max(o => o.Value);

            return new Csv(app.CsvConfig)
                .AddRecord(1, "Fil", file.Name)
                .AddRecord(1, "Oprettelsesdato", app.CsvConfig.DateHelper.Now())
                .AddRecord(1, "FilGemt", file.Saved)
                .AddRecord(1, "AflæstMin", aflæstMin)
                .AddRecord(1, "AflæstMax", aflæstMax);
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
                    .AddCalcFirstNotNull("Målernummer")
                    .AddInfoFrom("Fra")
                    .AddInfoTo("Til")
                    .AddCalcSum("Energi_Værdi", null, true)
                    //.AddCalcFirstNotNull("Energi_Enhed")
                    //.AddCalcFirstNotNull("Energi_Type")
                    .AddCalcSum("Volumen_Værdi", null, true)
                    //.AddCalcFirstNotNull("Volumen_Enhed")
                    //.AddCalcFirstNotNull("Volumen_Type")
                    //.AddCalcAny("Timer_Værdi")
                    //.AddCalcAny("Timer_Enhed")
                    //.AddCalcAny("Timer_Type")
                    .AddCalcAverage("Fremløb_Værdi")
                    //.AddCalcFirstNotNull("Fremløb_Enhed")
                    //.AddCalcFirstNotNull("Fremløb_Type")
                    .AddCalcAverage("Returløb_Værdi");
                    //.AddCalcFirstNotNull("Returløb_Enhed")
                    //.AddCalcFirstNotNull("Returløb_Type")
                    //.AddCalcFirstNotNull("Energiartskode");
            return timeStack;
        }

        private void ToDb(Csv csvForbrugPrTime, string table, bool saveToDb, string columnId = "Id")
        {
            if (saveToDb)
                app.Mssql.MergeCsv(csvForbrugPrTime, table, columnId, false, true);
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