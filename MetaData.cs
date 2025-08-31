using System;
using System.Collections.Generic;

namespace Kcsv2Bcr
{
    public class MetaData
    {
        // properties for use in the ISO 25178-71 file header
        public DateTime ModDate { get; } = DateTime.UtcNow;
        public DateTime CreateDate => GetCreateDate();
        public string ManufacID => GetRawValue("Model", "<unknown>");
        public int NumPoints => GetNumPoints();
        public int NumProfiles => GetNumProfiles();
        public double Xscale => GetLateralScale();
        public double Yscale => Xscale;
        public double Zscale => GetHeightScale();
        // properties for use in an ISO 25178-71 file trailer
        public string FileType => GetRawValue("DataType");
        public string FileTypeVersion => GetRawValue("FileVersion");
        public string RawFileName => GetRawValue("MeasurementDataName");
        public string SPMtechnique => GetRawValue("ScanMode");
        public string MeasurementMode => GetRawValue("MeasurementMode");
        public string LensMagnification => GetRawValue("LensPower");
        public string Resolution => GetRawValue("Resolution");
        public string ReferenceDatum => GetRawValue("ReferenceDataName");
        public string ZAxisSource => GetRawValue("OutputImageData");

        // additional properties
        public Dictionary<string, string> RawDictionary { get; } = new Dictionary<string, string>();
        public bool DelimiterReached { get; private set; } = false;
        public bool IsValid => IsMetadataValid();

        public void Add(string line) => Add(Helper.Tokenizer(line));

        private void Add(string[] tokens)
        {
            string[] translatedParts = Translate(tokens);
            switch (translatedParts.Length)
            {
                case 2:
                    // check if first token is empty
                    if (string.IsNullOrEmpty(translatedParts[0]))
                    {
                        RawDictionary["DataType"] = translatedParts[1];
                        break;
                    }
                    RawDictionary[translatedParts[0]] = translatedParts[1];
                    break;
                case 3:
                    RawDictionary[translatedParts[0]] = $"{translatedParts[1]} {translatedParts[2]}";
                    break; ;
                case 1:
                    // in the english version there is a line with "Data type" only
                    if (translatedParts[0].Contains("DataType"))
                    {
                        break;
                    }
                    DelimiterReached = true;
                    break;
                default:
                    break;
            }
        }

        private string GetRawValue(string key, string defaultValue)
        {
            if (RawDictionary.TryGetValue(key, out string value))
                return value;
            return defaultValue;
        }

        private string GetRawValue(string key) => GetRawValue(key, string.Empty);

        private int GetNumPoints() => Helper.MyInt(GetRawValue("Horizontal"));

        private int GetNumProfiles() => Helper.MyInt(GetRawValue("Vertical"));

        private double GetLateralScale()
        {
            double cellSize = double.NaN;
            double multiple = 1;
            string v = RawDictionary.ContainsKey("XYCalibration") ? RawDictionary["XYCalibration"] : "";
            string[] parts = v.Split(' ');
            if (parts.Length == 2)
            {
                cellSize = Helper.MyDouble(parts[0]);
                multiple = Helper.MyMultiple(parts[1]);
                return multiple * cellSize;
            }
            return double.NaN;
        }

        private double GetHeightScale() => Helper.MyMultiple(GetRawValue("Unit"));

        private DateTime GetCreateDate()
        {
            DateTime tempDT = ModDate;
            string value;
            if (RawDictionary.TryGetValue("CreateDate", out value))
            {
                DateTime.TryParse(value, out tempDT);
            }
            return tempDT;
        }

        private string[] Translate(string[] tokens)
        {
            string[] translated = new string[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                translated[i] = Translator.NativeWord(tokens[i]);
            }
            return translated;
        }

        private bool IsMetadataValid()
        {
            if (NumPoints <= 0)
                return false;
            if (NumProfiles <= 0)
                return false;
            if (Xscale <= 0)
                return false;
            if (double.IsNaN(Xscale))
                return false;
            return true;
        }

    }
}
