using System.Collections.Generic;

namespace Kcsv2Bcr
{
    public static class Translator
    {
        // the korean characters are double-byte characters and do not work here.
        private static readonly Dictionary<string, string> dict = new Dictionary<string, string>()
        {
            { "Gemessenes Datum",      "CreateDate" },
            { "Measured date",         "CreateDate" },            
            { "측정 일시",              "CreateDate" },
            { "Model",                 "Model" },
            { "Modell",                "Model" },
            { "기종",                   "Model" },
            { "Dateityp",              "DataType" },
            { "Data type",             "DataType" },
            { "파일 종류",              "DataType" },
            { "Dateiversion",          "FileVersion" },
            { "File version",          "FileVersion" },
            { "파일 버전",              "FileVersion" },
            { "Messdatenname",         "MeasurementDataName" },
            { "Measurement data name", "MeasurementDataName" },
            { "측정 데이터명",           "MeasurementDataName" },
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
            { "XY캘리브레이션",          "XYCalibration" },
            { "Bilddatenausg.",        "OutputImageData" },
            { "Output image data",     "OutputImageData" },
            { "출력 이미지 데이터",      "OutputImageData" },
            { "Horizont.",             "Horizontal" },
            { "Horizontal",            "Horizontal" },
            { "가로",                   "Horizontal" },
            { "Vertikal",              "Vertical" },
            { "Vertical",              "Vertical" },
            { "세로",                   "Vertical" },
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
            { "높이",                   "Height" },

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
