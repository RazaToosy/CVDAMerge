using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cvdamerge.Core.Models
{
    public class ModelCVDA
    {
        public string UniqueIdentifier { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Age { get; set; }
        public string MetricShortName { get; set; }
    }
}
