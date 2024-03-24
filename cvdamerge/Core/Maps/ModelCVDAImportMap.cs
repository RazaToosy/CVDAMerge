using CsvHelper.Configuration;
using cvdamerge.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cvdamerge.Core.Maps
{
    public sealed class ModelCVDAImportMap : ClassMap<ModelCVDA>
    {
        public ModelCVDAImportMap()
        {
            Map(m => m.UniqueIdentifier).Name("UniqueIdentifier");
            Map(m => m.Firstname).Name("Firstname");
            Map(m => m.Lastname).Name("Lastname");
            Map(m => m.Age).Name("Age");
            // Handling the space in the CSV column name for MetricShortName
            Map(m => m.MetricShortName).Name("Metric Short Name");
        }
    }
}
