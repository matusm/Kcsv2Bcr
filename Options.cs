using CommandLine;

namespace Kcsv2Bcr
{
    class Options
    {
        [Option("comment", Default = "---", HelpText = "User supplied comment string.")]
        public string UserComment { get; set; }

        [Option('m', "mask", Default = 0, HelpText = "Replace (mask) missing data points.")]
        public int ReplaceType { get; set; }        
        
        [Option('q', "quiet", HelpText = "Quiet mode. No screen output (except for errors).")]
        public bool BeQuiet { get; set; }

        [Option("strict", HelpText = "Force standardized format.")]
        public bool Strict { get; set; }

        [Option("iso", HelpText = "Output file ISO 25178-71:2012 compliant.")]
        public bool IsoFormat { get; set; }

        [Value(0, MetaName = "InputPath", Required = true, HelpText = "Input file-name including path")]
        public string InputPath { get; set; }

        [Value(1, MetaName = "OutputPath", HelpText = "Output file-name including path")]
        public string OutputPath { get; set; }

        public bool ReplaceNaN => GetReplace();

        private bool GetReplace()
        {
            if (ReplaceType > 0 && ReplaceType < 6)
                return true;
            return false;
        }

    }
}   
