namespace Meyer.Socrates.Data.Sections
{
    using System;
    using System.Linq;

    /// <summary>
    /// Image section used for various backgrounds and GUI elements.
    /// </summary>
    [SectionKey("MBMP")]
    public sealed class MBMP: SharedMBMP
    {
        public MBMP() : base(1, 1, false)
        { }

        public MBMP(int width, int height) : base(width, height, false)
        {
        }
    }
}
