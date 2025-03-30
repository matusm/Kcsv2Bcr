using System;

namespace Kcsv2Bcr
{
    public static class Helper
    {
        public static int MyInt(string token)
        {
            int number = -1;
            int.TryParse(token, out number);
            return number;
        }

        public static double MyDouble(string token)
        {
            double value;
            if (double.TryParse(token, out value))
                return value;
            else
                return double.NaN;
        }

        public static double MyMultiple(string token)
        {
            switch (token)
            {
                case "nm":
                    return 1e-9;
                case "µm":
                    return 1e-6;
                case "mm":
                    return 1e-3;
                case "cm":
                    return 1e-2;
                case "dm":
                    return 1e-1;
                case "km":
                    return 1e+3;
                default:
                    return 1;
            }
        }

        public static string[] Tokenizer(string line)
        {
            char[] charSeparators = { ',' };
            string[] tokens = StripQuotes(line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries));
            return tokens;
        }

        public static string[] StripQuotes(string[] tokens)
        {
            string[] newTokens = new string[tokens.Length];
            char[] trimChars = { '"' };
            for (int i = 0; i < tokens.Length; i++)
            {
                newTokens[i] = tokens[i].Trim(trimChars);
            }
            return newTokens;
        }
    }
}
