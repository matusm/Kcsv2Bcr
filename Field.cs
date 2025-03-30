using At.Matus.StatisticPod;

namespace Kcsv2Bcr
{
    public class Field
    {
        public double[,] Values { get; }
        public int NumPoints { get; }
        public int NumProfiles { get; }
        public int NumMissing => (NumPoints * NumProfiles) - (int)sp.SampleSize;
        public double MinHeight => sp.MinimumValue;
        public double MaxHeight => sp.MaximumValue;
        public double AverageHeight => sp.AverageValue;
        public double CentralHeight => sp.CentralValue;

        public Field(int points, int profiles)
        {
            NumPoints = points;
            NumProfiles = profiles;
            Values = new double[points, profiles];
            runningProfile = 0;
        }

        public void AddProfile(string[] tokens)
        {
            if (tokens.Length != NumPoints)
                return;
            if (runningProfile >= NumProfiles)
                return;
            for (int i = 0; i < NumPoints; i++)
            {
                double height = Helper.MyDouble(tokens[i]);
                Values[i, runningProfile] = height;
                sp.Update(height);
            }
            runningProfile++;
        }

        public double[] GetTopographyData() => GetTopographyData(1);

        public double[] GetTopographyData(double factor)
        {
            double[] data = new double[NumPoints * NumProfiles];
            for (int k = 0; k < NumProfiles; k++)
            {
                for (int i = 0; i < NumPoints; i++)
                {
                    data[i + k * NumPoints] = factor * Values[i, k];
                }
            }
            return data;
        }

        // profileIdx 1 ... NumProfiles !
        public double[] GetProfileData(int profileIdx, double factor)
        {
            double[] data = new double[NumPoints];
            if (profileIdx < 1) profileIdx = 1;
            if (profileIdx > NumProfiles) profileIdx = NumProfiles;
            for (int i = 0; i < NumPoints; i++)
            {
                data[i] = Values[i, profileIdx - 1];
            }
            return data;
        }

        public double[] GetProfileData(int profileIdx) => GetProfileData(profileIdx, 1);

        public void Regularize(double x)
        {
            if (NumMissing == 0)
                return;
            for (int k = 0; k < NumProfiles; k++)
            {
                for (int i = 0; i < NumPoints; i++)
                {
                    if (double.IsNaN(Values[i, k]))
                        Values[i, k] = x; ;
                }
            }
        }

        private int runningProfile;
        private readonly StatisticPod sp = new StatisticPod();
    }
}
