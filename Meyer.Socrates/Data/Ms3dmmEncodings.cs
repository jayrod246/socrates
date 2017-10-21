namespace Meyer.Socrates.Data
{
    using System;
    using System.Linq;

    public static class Ms3dmmEncodings
    {
        public const ushort Default = 0x0303;
        public const ushort Unicode = 0x0505;

        public static ushort GetSuitableEncoding(string str)
        {
            if (str == null) throw new ArgumentNullException("str");
            return str.Any(c => c > byte.MaxValue) ? Unicode : Default;
        }
    }
}
