namespace Meyer.Socrates.Data
{
    public interface IBitmapCanvas
    {
        int Width { get; set; }
        int Height { get; set; }
        int Stride { get; }
        PixelFormat PixelFormat { get; }
        byte[] GetPixelBuffer();
        int GetPixel(int x, int y);
        void SetPixelBuffer(byte[] buffer);
        void SetPixel(int x, int y, int value);
        void CopyTo(IBitmapCanvas dest, int destX, int destY);
        void Clear(int fill);
    }
}
