using Bygdrift.Tools.CsvTool;
using Bygdrift.Warehouse;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Module.Refines.Helpers
{
    /// <summary>
    /// Translates KMD's weird standard 'EK109' into correct CSV
    /// </summary>
    public class EK109
    {
        private static readonly CultureInfo pointCulture = new("en-US");
        private readonly AppBase app;
        private readonly Config csvConfig;

        public EK109(AppBase app, Config csvConfig)
        {
            this.app = app;
            this.csvConfig = csvConfig;
        }

        /// <summary>
        /// Translates KMD's weird standard 'EK109' into correct CSV
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="addId">Adds a column of data and meteringId</param>
        public Csv ToCsv(Stream stream, bool addId)
        {
            if (stream == null || stream.Length == 0)
                return null;

            var dataSets = StreamToEK109(stream);
            return Ek109ToCsv(dataSets, addId);
        }

        private Csv Ek109ToCsv(List<EK109DataSet> dataSets, bool addId)
        {
            var csv = new Csv(csvConfig);
            if (addId)
                csv.AddHeader("Id");

            csv.AddHeaders("Installation, Målernummer, Energiartskode, Aflæst, GældendeFra, Note");
            var headers = new Dictionary<int, (int Col, string Type)>();
            int col = addId ? 7 : 6;

            foreach (var dataSet in dataSets)
            {
                foreach (var header in dataSet.Headers)
                    if (!headers.ContainsKey(header.Art))
                    {
                        headers.Add(header.Art, (++col, header.Type));
                        csv.AddHeader(col, $"{header.Navn}_Værdi");
                        csv.AddHeader(++col, $"{header.Navn}_Enhed");
                        csv.AddHeader(++col, $"{header.Navn}_Type");
                    }

                foreach (var dRow in dataSet.Rows)
                {
                    if (dRow.Values.Sum(o => o.Værdi) > 0)  //If all values are 0, then there's an error and no reason to use it
                    {
                        var c = 1;
                        var row = new Dictionary<int, object>();
                        if (addId)
                            row.Add(c++, dRow.Målernummer + "-" + dRow.Aflæst.ToString("yyyyMMddTHHmmss"));

                        row.Add(c++, dRow.Installation);
                        row.Add(c++, dRow.Målernummer);
                        row.Add(c++, dRow.Energiartskode);
                        row.Add(c++, dRow.Aflæst);
                        row.Add(c++, dRow.GældendeFra);
                        row.Add(c++, dRow.Note);

                        foreach (var value in dRow.Values)
                        {
                            var lookedUpCol = headers[value.Art];
                            row.Add(lookedUpCol.Col, value.Værdi);
                            row.Add(lookedUpCol.Col + 1, value.Enhed);
                            row.Add(lookedUpCol.Col + 2, lookedUpCol.Type);
                        }

                        csv.AddRow(row);
                    }
                }
            }
            return csv;
        }

        private static List<EK109DataSet> StreamToEK109(Stream stream)
        {
            var dataSets = new List<EK109DataSet>();
            stream.Position = 0;
            var reader = new StreamReader(stream, leaveOpen: true);

            string line;
            var headers = new List<EK109Header>();
            var rows = new List<EK109Row>();

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("#"))
                {
                    if (headers.Any())
                    {
                        dataSets.Add(new(headers, rows));
                        rows = new();
                    }

                    headers = ReadHeader(line);
                }
                else
                    rows.Add(ReadRow(line));
            }
            return dataSets;
        }

        private static EK109Row ReadRow(string line)
        {
            var row = new EK109Row();
            if (string.IsNullOrEmpty(line))
                return default;

            var split = line.Split(';');
            row.Installation = split[0];
            row.Målernummer = split[1];
            row.EnergiArtKode = int.Parse(split[2]);
            row.Aflæst = DateTime.Parse(split[3]);
            row.GældendeFra = !string.IsNullOrEmpty(split[4]) ? DateTime.Parse(split[4]) : null;
            row.Note = split[5];
            for (int i = 6; i < split.Length; i++)
            {
                var kind = int.Parse(split[i++]);
                double.TryParse(split[i++], NumberStyles.Any, pointCulture, out double value);
                var unit = split[i];
                row.Values.Add((kind, value, unit));
            }

            return row;
        }

        private static List<EK109Header> ReadHeader(string line)
        {
            var headers = new List<EK109Header>();
            if (string.IsNullOrEmpty(line))
                return headers;

            var split = line.Split(';');

            for (int col = 6; col < split.Length; col++)
            {
                var kind = int.Parse(split[col++]);
                var name = split[col++];
                var type = split[col];
                headers.Add(new(kind, name, type));
            }

            return headers;
        }

    }

    internal class EK109DataSet
    {
        public List<EK109Header> Headers { get; set; }

        public List<EK109Row> Rows { get; set; }

        public EK109DataSet(List<EK109Header> headers, List<EK109Row> rows)
        {
            Headers = headers;
            Rows = rows;
        }
    }

    internal class EK109Header
    {
        public int Art { get; set; }
        public string Navn { get; set; }
        public string Type { get; set; }

        public EK109Header(int art, string navn, string type)
        {
            Art = art;
            Navn = navn;
            Type = type;
        }
    }

    internal class EK109Row
    {
        public string Installation { get; set; }
        public string Målernummer { get; set; }
        public int EnergiArtKode { get; set; }
        public DateTime Aflæst { get; set; }
        public DateTime? GældendeFra { get; set; }
        public string Note { get; set; }

        public List<(int Art, double Værdi, string Enhed)> Values { get; set; }
        public object Energiartskode { get; internal set; }

        public EK109Row()
        {
            Values = new();
        }
    }
}