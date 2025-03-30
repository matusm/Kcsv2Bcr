using System;
using System.Collections.Generic;

namespace Kcsv2Bcr
{
    public class MetaData
    {
        // properties for use in the ISO 25178-71 file header
        public DateTime ModDate { get; } = DateTime.UtcNow;
        public DateTime CreateDate => GetCreateDate();
        public string ManufacID => GetManufacturer();
        public int NumPoints => GetNumPoints();
        public int NumProfiles => GetNumProfiles();
        public double Xscale => GetLateralScale();
        public double Yscale => Xscale;
        public double Zscale => GetHeightScale();
        // properties for use in an ISO 25178-71 file trailer
        public string FileType { get; private set; } = string.Empty;
        public string FileTypeVersion { get; private set; } = string.Empty;
        public string RawFileName { get; private set; } = string.Empty;
        public string SPMtechnique { get; private set; } = string.Empty;
        public string MeasurementMode { get; private set; } = string.Empty;
        public string LensMagnification { get; private set; } = string.Empty;
        public string Resolution { get; private set; } = string.Empty;
        public string ReferenceDatum { get; private set; } = string.Empty;
        public string ZAxisSource { get; private set; } = string.Empty;
        
        // additional properties
        public Dictionary<string, string> RawDictionary { get; } = new Dictionary<string, string>();
        public bool DelimiterReached { get; private set; } = false;
        public bool IsValid => CheckData();

        public void Add(string[] tokens)
        {
            switch (tokens.Length)
            {
                case 2:
                    //RawDictionary.Add(tokens[0], tokens[1]);
                    RawDictionary[tokens[0]] = tokens[1];
                    InterpretDict(tokens[0], tokens[1]);
                    break;
                case 3:
                    //RawDictionary.Add(tokens[0] + "_A", tokens[1]);
                    //RawDictionary.Add(tokens[0] + "_B", tokens[2]);
                    RawDictionary[tokens[0] + "_A"] = tokens[1];
                    RawDictionary[tokens[0] + "_B"] = tokens[2];
                    break; ;
                case 1:
                    DelimiterReached = true;
                    break;
                default:
                    break;
            }
        }

        private void InterpretDict(string key, string value)
        {
            switch (key)
            {
                case "Dateityp":
                    FileType = value;
                    break;
                case "Dateiversion":
                    FileTypeVersion = value;
                    break;
                case "Messdatenname":
                    RawFileName = value;
                    break;
                case "Objektivvergrößerung":
                    LensMagnification = value;
                    break;
                case "Messmethode":
                    SPMtechnique = value;
                    if (value.Contains("Konfokaler Laser"))
                        SPMtechnique = "LaserConfocalScanning";
                    if (value.Contains("Weißlichtinterferometrie"))
                        SPMtechnique = "WhiteLightInterferometry";
                    if (value.Contains("Variation"))
                        SPMtechnique = "FocusVariation";
                    break;
                case "Auflösung":
                    Resolution = value; // this needs translation
                    break;
                case "Messmodus":
                    MeasurementMode = value; // this needs translation
                    break;
                case "Referenzdatenname":
                    ReferenceDatum = value;
                    break;
                case "Bilddatenausg.":
                    ZAxisSource = value;
                    if(value.Contains("Höhe"))
                        ZAxisSource = "Height";
                    break;
                default:
                    break;
            }
        }

        private bool CheckData()
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

        private int GetNumPoints()
        {
            string key = "Horizont.";
            string value;
            if (RawDictionary.TryGetValue(key, out value))
                return Helper.MyInt(value);
            else
                return -1;
        }

        private int GetNumProfiles()
        {
            string key = "Vertikal";
            string value;
            if (RawDictionary.TryGetValue(key, out value))
                return Helper.MyInt(value);
            else
                return -1;
        }

        private double GetLateralScale()
        {
            double cellSize = double.NaN;
            double multiple = 1;
            string value;
            if (RawDictionary.TryGetValue("XY-Justierung_A", out value))
                cellSize = Helper.MyDouble(value);
            if (RawDictionary.TryGetValue("XY-Justierung_B", out value))
                multiple = Helper.MyMultiple(value);
            return multiple * cellSize;
        }

        private double GetHeightScale()
        {
            double multiple = 1;
            string value;
            if (RawDictionary.TryGetValue("Einheit", out value))
                multiple = Helper.MyMultiple(value);
            return multiple;
        }

        private string GetManufacturer()
        {
            if (RawDictionary.TryGetValue("Modell", out string value))
                return (value);
            return "<unknown>";
        }

        private DateTime GetCreateDate()
        {
            DateTime tempDT = ModDate;
            string value;
            if (RawDictionary.TryGetValue("Gemessenes Datum", out value))
            {
                DateTime.TryParse(value, out tempDT);
            }
            return tempDT;
        }

    }
}
