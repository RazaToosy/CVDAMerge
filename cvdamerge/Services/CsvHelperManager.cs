using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using cvdamerge.Core.Maps;
using cvdamerge.Core.Models;

namespace cvdamerge.Services
{
    internal class CsvHelperManager
    {
        public List<ModelCVDA> ReadCsvFile(string filePath)
        {
            List<ModelCVDA> records;

            // Read all lines of the file
            var lines = File.ReadAllLines(filePath).ToList();

            // Find the index of the last line that is considered "blank" because it contains only commas
            int lastBlankLineIndex = lines.FindLastIndex(line => line.Trim().All(ch => ch == ','));

            // If a blank line is found and it's not the last line of the file
            if (lastBlankLineIndex != -1 && lastBlankLineIndex != lines.Count - 1)
            {
                // Keep only the lines above the last blank line
                var filteredLines = lines.Take(lastBlankLineIndex).ToList();

                // Overwrite the original file with the filtered content
                File.WriteAllLines(filePath, filteredLines);
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                IgnoreBlankLines = true, // Explicitly set to skip blank lines
                // Other configuration options as needed
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<ModelCVDAImportMap>();
                records = csv.GetRecords<ModelCVDA>().ToList();
            }

            return records;
        }

        public void WriteCsvFile(List<ModelCVDA> records, string filePath)
        {

            var mergedRecords = records
                .GroupBy(p => new { p.UniqueIdentifier, p.Firstname, p.Lastname, p.Age })
                .Select(g => new ModelCVDA
                {
                    UniqueIdentifier = g.Key.UniqueIdentifier,
                    Firstname = g.Key.Firstname,
                    Lastname = g.Key.Lastname,
                    Age = g.Key.Age,
                    MetricShortName = string.Join(", ", g.Select(x => x.MetricShortName))
                })
                .ToList();

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<ModelCVDAExportMap>();
                csv.WriteRecords(mergedRecords);
            }
        }
    }
}
