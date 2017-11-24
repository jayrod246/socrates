namespace Meyer.Socrates.Data
{
    using System;
    using System.Globalization;

    public class VersionInfo
    {
        public static readonly VersionInfo English = new VersionInfo("English", Ms3dmm.MAGIC_NUM_US, CultureInfo.GetCultureInfo(1033));
        public static readonly VersionInfo Japanese = new VersionInfo("Japanese", Ms3dmm.MAGIC_NUM_JP, CultureInfo.GetCultureInfo(1041));
        public string Name { get; }
        public uint MagicNumber { get; }
        public CultureInfo CultureInfo { get; }

        private VersionInfo(string name, uint magicNumber, CultureInfo cultureInfo)
        {
            Name = name;
            MagicNumber = magicNumber;
            CultureInfo = cultureInfo;
        }

        public static VersionInfo GetVersionInfo(uint magicNumber)
        {
            switch (magicNumber)
            {
                case Ms3dmm.MAGIC_NUM_US:
                    return English;
                case Ms3dmm.MAGIC_NUM_JP:
                    return Japanese;
                default: throw new ArgumentException("VersionInfo could not be resolved.", "magicNumber");
            }
        }

        public static bool TryGetVersionInfo(uint magicNumber, out VersionInfo versionInfo)
        {
            try
            {
                versionInfo = GetVersionInfo(magicNumber);
                return true;
            }
            catch
            {
                versionInfo = null;
                return false;
            }
        }
    }
}
