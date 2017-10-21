namespace Meyer.Socrates.Data
{
    public interface IIndexed8Image: IBitmapCanvas
    {
        void SetPixel(int x, int y, byte value);
        void Clear(byte fill);
    }
}

