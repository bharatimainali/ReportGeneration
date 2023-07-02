using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ReportGeneration.Model
{
    [XmlRoot("GenerationOutput")]
    public class GenerationOutput
    {
        [XmlArray("Totals")]
        [XmlArrayItem("Generator")]
        public List<GeneratorTotal> Totals { get; set; }

        [XmlArray("MaxEmissionGenerators")]
        [XmlArrayItem("Day")]
        public List<DailyMaxEmission> MaxEmissionGenerators { get; set; }

        [XmlArray("ActualHeatRates")]
        [XmlArrayItem("CoalGenerator")]
        public List<CoalHeatRate> ActualHeatRates { get; set; }
    }

    public class GeneratorTotal
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Total")]
        public double Total { get; set; }
    }
}
