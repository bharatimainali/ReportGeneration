using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;
using ReportGeneration.Model;

namespace ReportGeneration
{
    public class Program
    {
        private static IConfiguration configuration { get; set; }

        public static void Main(string[] args)
        {
            // getting the settings from appsettings.json
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            configuration = builder.Build();

            // initilizing the FileSystemWatcher through MonitorDirectory method
            var inputPath = configuration.GetSection("inputXmlPath").Value;
            MonitorDirectory(inputPath);

            // displaying a message to stop the application to not exit
            Console.Write("press any key and enter to exit the application: ");
            Console.ReadLine();
        }

        // MonitorDirectory method will initilize the FileSystemWatcher
        // FileSystemWatcher will monitor the input folder for any file
        private static void MonitorDirectory(string path)
        {
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = path;
            fileSystemWatcher.Created += InitFileWatcher;
            fileSystemWatcher.Changed += InitFileWatcher;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        // InitFileWatcher is set already in the FileSystemWatcher.Created and FileSystemWatcher.Changed
        // InitFileWatcher will process the file by using ProcessXmlFile
        private static void InitFileWatcher(object sender, FileSystemEventArgs e)
        {
            ProcessXmlFile(e.FullPath);
        }

        // ProcessXmlFile
        // 1. read the xml files (input file and the reference data file) and store them into objects
        // 2. perform calculations
        // 3. make the output file and store it into output file location
        private static void ProcessXmlFile(string inputFolderPath)
        {
            // Load the files and store them into varibles
            var inputFilePath = inputFolderPath;
            var outputFilePath = configuration.GetSection("outputXmlPath").Value + "\\GenerationOutput.xml";
            var refFilePath = configuration.GetSection("referenceDataPath").Value + "\\ReferenceData.xml";

            using var inputReportReader = new StreamReader(inputFilePath);
            var serializer = new XmlSerializer(typeof(GenerationReport));
            var inputGenerationReport = (GenerationReport)serializer.Deserialize(inputReportReader);

            using var refReader = new StreamReader(refFilePath);
            serializer = new XmlSerializer(typeof(ReferenceData));
            var referenceData = (ReferenceData)serializer.Deserialize(refReader);

            var generatorTotals = new List<GeneratorTotal>();
            var dailyMaxEmissions = new List<DailyMaxEmission>();
            var coalHeatRates = new List<CoalHeatRate>();

            // Perfoming the calculations
            foreach (var windGenerator in inputGenerationReport.Wind.WindGenerators)
            {
                generatorTotals.Add(new GeneratorTotal { Name = windGenerator.Name, Total = windGenerator.CalculateTotal(referenceData) });
            }

            foreach (var gasGenerator in inputGenerationReport.Gas.GasGenerators)
            {
                generatorTotals.Add(new GeneratorTotal { Name = gasGenerator.Name, Total = gasGenerator.CalculateTotal(referenceData) });
                dailyMaxEmissions.AddRange(gasGenerator.CalculateDailyEmissions(referenceData));
            }

            foreach (var coalGenerator in inputGenerationReport.Coal.CoalGenerators)
            {
                generatorTotals.Add(new GeneratorTotal { Name = coalGenerator.Name, Total = coalGenerator.CalculateTotal(referenceData) });
                dailyMaxEmissions.AddRange(coalGenerator.CalculateDailyEmissions(referenceData));
                coalHeatRates.Add(coalGenerator.CalculateHeatRate());
            }

            var maxEmissionGenerators = dailyMaxEmissions
                .GroupBy(e => e.Date)
                .Select(g => g.OrderByDescending(e => e.Emission).First())
                .ToList();

            // Create the output object
            var generationOutput = new GenerationOutput
            {
                Totals = generatorTotals,
                MaxEmissionGenerators = maxEmissionGenerators,
                ActualHeatRates = coalHeatRates
            };

            // Store the object (generationOutput) into an output xml file 
            serializer = new XmlSerializer(typeof(GenerationOutput));
            using var writer = new StreamWriter(outputFilePath);
            serializer.Serialize(writer, generationOutput);
        }
    }
}
