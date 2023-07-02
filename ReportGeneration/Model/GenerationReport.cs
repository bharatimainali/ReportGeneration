using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ReportGeneration.Model
{
    [XmlRoot("GenerationReport")]
    public class GenerationReport
    {
        [XmlElement("Wind")]
        public Wind Wind { get; set; }

        [XmlElement("Gas")]
        public Gas Gas { get; set; }

        [XmlElement("Coal")]
        public Coal Coal { get; set; }
    }

    public class Wind
    {
        [XmlElement("WindGenerator")]
        public List<WindGenerator> WindGenerators { get; set; }
    }

    public class Gas
    {
        [XmlElement("GasGenerator")]
        public List<GasGenerator> GasGenerators { get; set; }
    }

    public class Coal
    {
        [XmlElement("CoalGenerator")]
        public List<CoalGenerator> CoalGenerators { get; set; }
    }

    public class WindGenerator
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Generation")]
        public Generation Generation { get; set; }

        [XmlElement("Location")]
        public string Location { get; set; }

        public double CalculateTotal(ReferenceData referenceData)
        {
            double total = 0;

            double valueFactor = (Location == "Offshore") ? referenceData.Factors.ValueFactor.Low : referenceData.Factors.ValueFactor.High;

            foreach (var day in Generation.Days)
            {
                total += (double)day.Energy * (double)day.Price * valueFactor;
            }

            return total;
        }
    }

    public class GasGenerator
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Generation")]
        public Generation Generation { get; set; }

        [XmlElement("EmissionsRating")]
        public decimal EmissionsRating { get; set; }

        public double CalculateTotal(ReferenceData referenceData)
        {
            double total = 0;
            double valueFactor = referenceData.Factors.ValueFactor.Medium;

            foreach (var day in Generation.Days)
            {
                total += (double)day.Energy * (double)day.Price * valueFactor;
            }
            return total;
        }

        public List<DailyMaxEmission> CalculateDailyEmissions(ReferenceData referenceData)
        {
            var dailyEmissions = new List<DailyMaxEmission>();
            double emissionsFactor = referenceData.Factors.EmissionsFactor.Medium;

            foreach (var day in Generation.Days)
            {
                var emission = (double)day.Energy * (double)EmissionsRating * emissionsFactor;
                dailyEmissions.Add(new DailyMaxEmission { Name = Name, Date = day.Date, Emission = emission });
            }

            return dailyEmissions;
        }
    }

    public class CoalGenerator
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Generation")]
        public Generation Generation { get; set; }

        [XmlElement("TotalHeatInput")]
        public decimal TotalHeatInput { get; set; }

        [XmlElement("ActualNetGeneration")]
        public decimal ActualNetGeneration { get; set; }

        [XmlElement("EmissionsRating")]
        public decimal EmissionsRating { get; set; }

        public double CalculateTotal(ReferenceData referenceData)
        {
            double total = 0;
            double valueFactor = referenceData.Factors.ValueFactor.Medium;

            foreach (var day in Generation.Days)
            {
                total += (double)day.Energy * (double)day.Price * valueFactor;
            }
            return total;
        }

        public List<DailyMaxEmission> CalculateDailyEmissions(ReferenceData referenceData)
        {
            var dailyEmissions = new List<DailyMaxEmission>();
            double emissionsFactor = referenceData.Factors.EmissionsFactor.High;

            foreach (var day in Generation.Days)
            {
                var emission = (double)day.Energy * (double)EmissionsRating * emissionsFactor;
                dailyEmissions.Add(new DailyMaxEmission { Name = Name, Date = day.Date, Emission = emission });
            }
            return dailyEmissions;
        }

        public CoalHeatRate CalculateHeatRate()
        {
            var heatRate = new CoalHeatRate
            {
                Name = Name,
                HeatRate = (double)TotalHeatInput / (double)ActualNetGeneration
            };
            return heatRate;
        }
    }

    public class Generation
    {
        [XmlElement("Day")]
        public List<Day> Days { get; set; }
    }

    public class Day
    {
        [XmlElement("Date")]
        public DateTime Date { get; set; }

        [XmlElement("Energy")]
        public decimal Energy { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }
    }

    public class DailyMaxEmission
    {
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public double Emission { get; set; }
    }

    public class CoalHeatRate
    {
        public string Name { get; set; }
        public double HeatRate { get; set; }
    }

}
