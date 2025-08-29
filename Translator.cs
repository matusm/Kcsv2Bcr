using System.Collections.Generic;

namespace Kcsv2Bcr
{
    public static class Translator
    {
        private static readonly Dictionary<string, string> dict = new Dictionary<string, string>()
        {
            { "Gemessenes Datum",      "CreateDate" },
            { "Measured date",         "CreateDate" },
            { "Model",                 "Model" },
            { "Modell",                "Model" },
            { "Dateityp",              "DataType" },
            { "Data type",             "DataType" },
            { "Dateiversion",          "FileVersion" },
            { "File version",          "FileVersion" },
            { "Messdatenname",         "MeasurementDataName" },
            { "Measurement data name", "MeasurementDataName" },
            { "Auflösung",             "Resolution" },
            { "Resolution",            "Resolution" },
            { "Messmodus",             "MeasurementMode" },
            { "Measurement Mode",      "MeasurementMode" },
            { "Messmethode",           "ScanMode" },
            { "Scan Mode",             "ScanMode" },
            { "Objektivvergrößerung",  "LensPower" },
            { "Objective Lens Power",  "LensPower" },
            { "XY-Justierung",         "XYCalibration" },
            { "XY Calibration",        "XYCalibration" },
            { "Bilddatenausg.",        "OutputImageData" },
            { "Output image data",     "OutputImageData" },
            { "Horizont.",             "Horizontal" },
            { "Horizontal",            "Horizontal" },
            { "Vertikal",              "Vertical" },
            { "Vertical",              "Vertical" },
            { "Mindestwert",           "MinimumValue" },
            { "Minimum value",         "MinimumValue" },
            { "Höchstwert",            "MaximumValue" },
            { "Maximum value",         "MaximumValue" },
            { "Einheit",               "Unit" },
            { "Unit",                  "Unit" },
            { "Referenzdatenname",     "ReferenceDataName" },
            { "Reference data name",   "ReferenceDataName" },
            { "Höhe",                  "Height" },
            { "Height",                "Height" },

            { "Laser confocal",             "LaserConfocalScanning" },
            { "Konfokaler Laser",           "LaserConfocalScanning" },
            { "Fokusvariation",             "FocusVariation" },
            { "Variation",                  "FocusVariation" },
            { "Weißlichtinterferometrie",   "WhiteLightInterferometry" },
            { "White light interferometry", "WhiteLightInterferometry" },
            { "Surface profile",            "SurfaceProfile" },
            { "Oberflächenprofil",          "SurfaceProfile" }
        };

        public static string NativeWord(string token)
        {
            if (dict.ContainsKey(token))
                return dict[token];
            else
                return token;
        }

    }
}
