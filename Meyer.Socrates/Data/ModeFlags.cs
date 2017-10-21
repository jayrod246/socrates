namespace Meyer.Socrates.Data
{
    using System;

    [Flags]
    public enum ModeFlags: byte
    {
        Uncompressed = 0,
        Main = 2,
        Compressed = 4,
        HelpUnk = 16,
        UncompressedMain = Main | Uncompressed,
        CompressedMain = Main | Compressed,
        Automatic = 128,
    }
}
