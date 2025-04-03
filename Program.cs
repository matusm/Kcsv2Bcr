using Bev.IO.BcrWriter;
using Bev.UI;
using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Kcsv2Bcr
{
    class Program
    {
        private static Options options = new Options(); // this must be set in Run()

        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Parser parser = new Parser(with => with.HelpWriter = null);
            ParserResult<Options> parserResult = parser.ParseArguments<Options>(args);
            parserResult
                .WithParsed<Options>(options => Run(options))
                .WithNotParsed(errs => DisplayHelp(parserResult, errs));
        }

        private static void Run(Options ops)
        {
            options = ops;
            if (options.BeQuiet == true)
                ConsoleUI.BeSilent();
            else
                ConsoleUI.BeVerbatim();
            ConsoleUI.Welcome();

            string inputFilename = Path.ChangeExtension(options.InputPath, "csv");
            string outputFilename = GetOutputFilename(inputFilename);
            
            MetaData metaData = new MetaData();

            using (new InfoOperation("Analyzing metadata"))
            {
                #region get metadata
                using (var reader = new StreamReader(inputFilename, Encoding.UTF7))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] tokens = Helper.Tokenizer(line);
                        metaData.Add(tokens);
                        if (metaData.DelimiterReached)
                            break;
                    }
                }
                if (!metaData.IsValid)
                {
                    ConsoleUI.ErrorExit("Metadata invalid!", 1);
                }
                #endregion
            }

            Field field = new Field(metaData.NumPoints, metaData.NumProfiles);

            using (new InfoFileRead(inputFilename))
            {
                #region get topography data
                using (var reader = new StreamReader(inputFilename, Encoding.UTF7))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] tokens = Helper.Tokenizer(line);
                        field.AddProfile(tokens);
                    }
                }
                #endregion
            }

            // replace invalid points
            if(options.ReplaceNaN)
            {
                field.Regularize(GetReplaceValueFor(options.ReplaceType));
                ConsoleUI.WriteLine($"{field.NumMissing} missing data points replaced by {GetReplaceValueFor(options.ReplaceType)}.");
            }

            #region BCR stuff
            BcrWriter bcr = new BcrWriter();
            bcr.Relaxed = !options.Strict; // overrules Relaxed
            ConsoleUI.WriteLine(bcr.Relaxed ? "Relaxed formatting" : "Strict formatting");
            bcr.ForceIsoFormat = options.IsoFormat;
            ConsoleUI.WriteLine(bcr.ForceIsoFormat ? "ISO 25178-71 format" : "Legacy format");
            
            // ISO 25178-71 file header
            bcr.ManufacurerId = metaData.ManufacID;
            bcr.CreationDate = metaData.CreateDate;
            bcr.ModificationDate = metaData.ModDate;
            bcr.NumberOfPointsPerProfile = field.NumPoints;
            bcr.NumberOfProfiles = field.NumProfiles;
            bcr.XScale = metaData.Xscale;
            bcr.YScale = metaData.Yscale;
            bcr.ZScale = metaData.Zscale;

            // ISO 25178-71 main section
            bcr.PrepareMainSection(field.GetTopographyData(metaData.Zscale));

            // ISO 25178-71 file trailer
            Dictionary<string, string> bcrMetaData = new Dictionary<string, string>();
            bcrMetaData.Add("UserComment", options.UserComment);
            bcrMetaData.Add("ConvertedBy", $"{HeadingInfo.Default}");
            bcrMetaData.Add("BcrWriter", $"{typeof(BcrWriter).Assembly.GetName().Name} {typeof(BcrWriter).Assembly.GetName().Version}");
            bcrMetaData.Add("InputFile", inputFilename);
            bcrMetaData.Add("RawFileName", metaData.RawFileName);
            bcrMetaData.Add("SPMtechnique", metaData.SPMtechnique);
            bcrMetaData.Add("MeasurementMode", metaData.MeasurementMode); 
            bcrMetaData.Add("ZAxisSource", metaData.ZAxisSource);
            bcrMetaData.Add("Resolution", metaData.Resolution);
            bcrMetaData.Add("LensMagnification", metaData.LensMagnification);
            bcrMetaData.Add("ReferenceDatum", metaData.ReferenceDatum);
            bcrMetaData.Add("MinimumValue", $"{field.MinHeight:F3} µm");
            bcrMetaData.Add("MaximumValue", $"{field.MaxHeight:F3} µm");
            if (field.NumMissing!=0)
            {
                bcrMetaData.Add("InvalidPoints", $"{field.NumMissing}");
                if(options.ReplaceNaN)
                    bcrMetaData.Add("InvalidPointsReplacedBy", $"{GetReplaceValueFor(options.ReplaceType):F3}");
            }
            // include RawDictonary?
            foreach (var entry in metaData.RawDictionary)
                bcrMetaData.Add($"[{entry.Key}]", $"[{entry.Value}]");

            bcr.PrepareTrailerSection(bcrMetaData);

            using(new InfoFileWrite(outputFilename))
            {
                bcr.WriteToFile(outputFilename);
            }
            #endregion

            double GetReplaceValueFor(int type)
            {
                switch (type)
                {
                    case 1:
                        return 0;
                    case 2:
                        return field.MinHeight;
                    case 3:
                        return field.MaxHeight;
                    case 4:
                        return field.AverageHeight;
                    case 5:
                        return field.CentralHeight;
                    default:
                        return double.NaN; // this should not happen
                }
            }

        }

        static string GetOutputFilename(string filename) => string.IsNullOrWhiteSpace(options.OutputPath) ? Path.ChangeExtension(filename, "sdf") : options.OutputPath;

        private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            string appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            HelpText helpText = HelpText.AutoBuild(result, h =>
            {
                h.AutoVersion = false;
                h.AdditionalNewLineAfterOption = false;
                h.AddPreOptionsLine("\nProgram to convert scanning files by Keyence VK X-3000 to BCR or ISO 25178 - 71:2012 raster data format. ");
                h.AddPreOptionsLine("");
                h.AddPreOptionsLine($"Usage: {appName} InputPath [OutPath] [options]");
                h.AddPostOptionsLine("");
                h.AddPostOptionsLine("Supported values for -m, --mask:");
                h.AddPostOptionsLine("   0: nop (keep invalid points)");
                h.AddPostOptionsLine("   1: replace all invalid points by 0");
                h.AddPostOptionsLine("   2: replace all invalid points by the minimum hight value");
                h.AddPostOptionsLine("   3: replace all invalid points by the maximum hight value");
                h.AddPostOptionsLine("   4: replace all invalid points by the average hight value");
                h.AddPostOptionsLine("   5: replace all invalid points by the central hight value");
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            Console.WriteLine(helpText);
        }

    }
}
