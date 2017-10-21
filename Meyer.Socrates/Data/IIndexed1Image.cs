
namespace Meyer.Socrates.Data
{
    public interface IIndexed1Image: IBitmapCanvas
    {
        void SetPixel(int x, int y, bool value);
        void Clear(bool fill);
    }
}

