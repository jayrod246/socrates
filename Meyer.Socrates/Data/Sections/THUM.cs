namespace Meyer.Socrates.Data.Sections
{
    [SectionKey("THUM")]
    public sealed class THUM: SharedMBMP
    {
        public THUM() : base(1, 1, true)
        {
        }

        public THUM(int width, int height) : base(width, height, true)
        {
        }
    }
}
