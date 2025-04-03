using System;
using System.Collections.Generic;

namespace Kcsv2Bcr
{
    public class MetaData
    {
        // properties for use in the ISO 25178-71 file header
        public DateTime ModDate { get; } = DateTime.UtcNow;
        public DateTime CreateDate { get; private set;} = DateTime.UtcNow;
        public string ManufacID { get; private set; } = "???";
        public int NumPoints { get; private set; } = -1;
        public int NumProfiles { get; private set; } = -1;
        public double Xscale { get; private set; } = double.NaN;
        public double Yscale => Xscale;
        public double Zscale { get; private set; } = double.NaN;
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
        public bool IsValid => IsMetadataValid();

        public void Add(string[] tokens)
        {
            switch (tokens.Length)
            {
                case 2:
                    RawDictionary[tokens[0]] = tokens[1];
                    InterpretTwoTokenLine(tokens[0], tokens[1]);
                    break;
                case 3:
                    RawDictionary[tokens[0]] = $"{tokens[1]} {tokens[2]}";
                    InterpretThreeTokenLine(tokens[0], tokens[1], tokens[2]);
                    break; ;
                case 1:
                    InterpretSingleTokenLine(tokens[0]);
                    break;
                default:
                    break;
            }
        }

        private void InterpretThreeTokenLine(string key, string v1, string v2)
        {
            switch (key)
            {
                case "XY Calibration":
                case "XY-Justierung":
                    Xscale = Helper.MyDouble(v1) * Helper.MyMultiple(v2);
                    break;
                default:
                    break;
            }
        }

        private void InterpretTwoTokenLine(string key, string value)
        {
            switch (key)
            {
                case "Horizontal":
                case "Horizont.":
                    NumPoints= Helper.MyInt(value);
                    break;
                case "Vertical":
                case "Vertikal":
                    NumProfiles = Helper.MyInt(value);
                    break;
                case "Unit":
                case "Einheit":
                    Zscale = Helper.MyMultiple(value);
                    break;
                case "Measured date":
                case "Gemessenes Datum":
                    DateTime.TryParse(value, out DateTime tempDT);
                    CreateDate = tempDT;
                    break;
                case "Model":
                case "Modell":
                    ManufacID = value;
                    break;
                case "Dateityp":
                case "Data type":   // there is a line break in the example file!
                    FileType = value;
                    break;
                case "Dateiversion":
                case "File version":
                    FileTypeVersion = value;
                    break;
                case "Messdatenname":
                case "Measurement data name":
                    RawFileName = value;
                    break;
                case "Objektivvergrößerung":
                case "Objective Lens Power":
                    LensMagnification = value;
                    break;
                case "Messmethode":
                case "Scan Mode":
                    SPMtechnique = value;
                    if (value.Contains("Konfokaler Laser"))
                        SPMtechnique = "Laser confocal";
                    if (value.Contains("Weißlichtinterferometrie"))
                        SPMtechnique = "WhiteLightInterferometry";
                    if (value.Contains("Variation"))
                        SPMtechnique = "FocusVariation";
                    break;
                case "Auflösung":
                case "Resolution":
                    Resolution = value; // this needs translation
                    break;
                case "Messmodus":
                case "Measurement Mode":
                    MeasurementMode = value; // this needs translation
                    break;
                case "Referenzdatenname":
                case "Reference data name":
                    ReferenceDatum = value;
                    break;
                case "Bilddatenausg.":
                case "Output image data":
                    ZAxisSource = value;
                    if(value.Contains("Höhe"))
                        ZAxisSource = "Height";
                    break;
                default:
                    break;
            }
        }

        private void InterpretSingleTokenLine(string key)
        {
            switch (key)
            {
                case "Data type":   // this is probably an error in the export routine!!
                    break;
                default:
                    DelimiterReached = true;
                    break;
            }
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
