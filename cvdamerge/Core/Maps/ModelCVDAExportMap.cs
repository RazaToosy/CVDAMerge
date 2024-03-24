using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using cvdamerge.Core.Models;

namespace cvdamerge.Core.Maps
{
    public sealed class  ModelCVDAExportMap :ClassMap<ModelCVDA>
    {
        public ModelCVDAExportMap()
        {
            Map(m => m.UniqueIdentifier).Name("NHS Number");
            Map(m => m.MetricShortName);
        }
    }
}
