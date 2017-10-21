namespace Meyer.Socrates.Data
{
    public enum CompressionType : uint
    {
        Uncompressed = 0,
        KCDC = 0x43_44_43_4B,
        KCD2 = 0x32_44_43_4B
    }
}
