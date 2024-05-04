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
            var lines = File.ReadAllLines(filePath);

            // Find the index of the last line that is considered "blank" because it contains only commas
            int lastBlankLineIndex = Array.FindLastIndex(lines, line => line.Trim().All(ch => ch == ','));

            // If a blank line is found and it's not the last line of the file
            if (lastBlankLineIndex != -1 && lastBlankLineIndex != lines.Length - 1)
            {
                // Keep only the lines above the last blank line
                var filteredLines = new string[lastBlankLineIndex];
                Array.Copy(lines, filteredLines, lastBlankLineIndex);

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
            // Dictionary to hold aggregated records by unique identifier
            var aggregatedRecords = new Dictionary<string, ModelCVDA>();

            // Aggregate records
            foreach (var record in records)
            {
                if (!aggregatedRecords.TryGetValue(record.UniqueIdentifier, out var aggregatedRecord))
                {
                    // If the record does not exist, add it to the dictionary
                    aggregatedRecord = new ModelCVDA
                    {
                        UniqueIdentifier = record.UniqueIdentifier,
                        Firstname = record.Firstname,
                        Lastname = record.Lastname,
                        Age = record.Age,
                        MetricShortName = ""
                    };
                    aggregatedRecords.Add(record.UniqueIdentifier, aggregatedRecord);
                }

                // Append metric short names, comma-separated
                if (!string.IsNullOrEmpty(aggregatedRecord.MetricShortName))
                {
                    aggregatedRecord.MetricShortName += ",";
                }
                aggregatedRecord.MetricShortName += record.MetricShortName;
            }

            // Extract unique metric short names for headers
            var metricShortNames = GetUniqueMetricShortNames(aggregatedRecords.Values.ToList());

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<ModelCVDAExportMap>();

                // Write the header row
                csv.WriteField("UniqueIdentifier");
                csv.WriteField("Firstname");
                csv.WriteField("Lastname");
                csv.WriteField("Age");
                csv.WriteField("MetricShortNames");
                foreach (var metricShortName in metricShortNames)
                {
                    csv.WriteField(metricShortName);
                }
                csv.NextRecord();

                // Write the data rows
                foreach (var record in aggregatedRecords.Values)
                {
                    csv.WriteField(record.UniqueIdentifier);
                    csv.WriteField(record.Firstname);
                    csv.WriteField(record.Lastname);
                    csv.WriteField(record.Age);
                    csv.WriteField(string.Join(",", record.MetricShortName.Split(',').Select(s => s.Trim())));

                    var metrics = record.MetricShortName.Split(',').Select(s => s.Trim()).ToList();
                    foreach (var metricShortName in metricShortNames)
                    {
                        // Write 1 if the metric short name is associated with the patient, otherwise empty
                        csv.WriteField(metrics.Contains(metricShortName) ? metricShortName : "");
                    }
                    csv.NextRecord();
                }
            }
        }

        private List<string> GetUniqueMetricShortNames(List<ModelCVDA> records)
        {
            var metricShortNames = new HashSet<string>(); // Use a HashSet to automatically avoid duplicates

            foreach (var record in records)
            {
                foreach (var metricShortName in record.MetricShortName.Split(','))
                {
                    metricShortNames.Add(metricShortName.Trim());
                }
            }

            return metricShortNames.ToList();
        }


    }
}
