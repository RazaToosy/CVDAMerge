using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using cvdamerge.Core.Models;

namespace cvdamerge.Data
{
    internal class Repo
    {
        public  List<string> Files { get; }
        public Dictionary<string, List<string>> CvdaTargets { get; }
        public List<ModelCVDA> CvdaModels { get; }

        public Repo(string folderRootPath)
        {
            Files = GetFiles(folderRootPath);
            CvdaTargets = importCVDATargets(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"CVDATargets.xml"));
            CvdaModels = new List<ModelCVDA>();
        }

        public List<string> GetFiles(string folderRootPath)
        {
            return Directory.GetFiles(folderRootPath, "*.csv").Select(Path.GetFileName).ToList();
        }

        private Dictionary<string, List<string>> importCVDATargets(string xmlString)
        {
            var xDocument = XDocument.Parse(File.ReadAllText(xmlString));
            var dictionary = new Dictionary<string, List<string>>();

            foreach (var condition in xDocument.Descendants("Condition"))
            {
                var conditionName = condition.Attribute("name")?.Value;
                var targetsList = new List<string>();

                foreach (var target in condition.Descendants("Target"))
                {
                    // Directly accessing .Value property handles CDATA sections appropriately
                    targetsList.Add(target.Value);
                }

                if (conditionName != null)
                {
                    dictionary[conditionName] = targetsList;
                }
            }
            return dictionary;
        }


    }
}
