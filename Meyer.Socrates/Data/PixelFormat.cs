namespace Meyer.Socrates.Data
{
    public enum PixelFormat
    {
        /// <summary>
        /// Paletted image, 256 colors.
        /// </summary>
        Indexed8 = 0x04,

        /// <summary>
        /// Monochrome, 2-color image, black and white only.
        /// </summary>
        BlackWhite = 0x5,

        /// <summary>
        /// Gray format, 16 bpp.
        /// </summary>
        Gray16 = 0xB,
    }
}
