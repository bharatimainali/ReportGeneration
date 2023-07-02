using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ReportGeneration.Model
{
    [XmlRoot("ReferenceData")]
    public class ReferenceData
    {
        [XmlElement("Factors")]
        public Factors Factors { get; set; }
    }

    public class Factors
    {
        [XmlElement("ValueFactor")]
        public Factor ValueFactor { get; set; }

        [XmlElement("EmissionsFactor")]
        public Factor EmissionsFactor { get; set; }
    }

    public class Factor
    {
        [XmlElement("High")]
        public double High { get; set; }

        [XmlElement("Medium")]
        public double Medium { get; set; }

        [XmlElement("Low")]
        public double Low { get; set; }
    }

}
