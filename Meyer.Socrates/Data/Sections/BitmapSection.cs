namespace Meyer.Socrates.Data.Sections
{
    using System;
    using System.Diagnostics.Contracts;

    public abstract class BitmapSection: VirtualSection, IBitmapCanvas
    {
        private byte[] pixels;

        public virtual int Width { get => GetValue<int>(); set => SetValue(value); }
        public virtual int Height { get => GetValue<int>(); set => SetValue(value); }
        public int Stride => CalculateStride(Width, PixelFormat); // Does not need RequireLoad because Width property is accessed.

        internal static int CalculateStride(int width, PixelFormat fmt)
        {
            int row_size;
            switch (fmt)
            {
                case PixelFormat.Indexed8:
                    row_size = width;
                    break;
                case PixelFormat.Gray16:
                    row_size = width * 2;
                    break;
                case PixelFormat.BlackWhite:
                    row_size = (width + 7) / 8;
                    break;
                default:
                    throw new ArgumentException("Stride could not be calculated due to an unknown pixel format.");
            }

            return (row_size + 3) & ~3;
        }

        public abstract PixelFormat PixelFormat { get; }

        internal protected byte[] PixelBuffer => GetPixelBuffer();

        public virtual byte[] GetPixelBuffer()
        {
            using (Lock())
            {
                return pixels;
            }
        }

        void IBitmapCanvas.SetPixel(int x, int y, int value)
        {
            using (Lock()) SetPixelCore(x, y, value);
        }

        protected virtual void SetPixelCore(int x, int y, int value)
        {
            int bytesPerPixel = 1;

            switch (PixelFormat)
            {
                case PixelFormat.Gray16:
                    bytesPerPixel = 2;
                    goto case PixelFormat.Indexed8;
                case PixelFormat.Indexed8:
                    int byteOffset = x * bytesPerPixel + y * Stride;
                    if (x < 0) throw new ArgumentOutOfRangeException("x is less than zero", "x");
                    if (y < 0) throw new ArgumentOutOfRangeException("y is less than zero", "y");
                    if (x > Width) throw new ArgumentOutOfRangeException("x is greater than width", "x");
                    if (y > Height) throw new ArgumentOutOfRangeException("y is greater than height", "y");
                    if (byteOffset + bytesPerPixel > pixels.Length)
                        throw new ArgumentException("x and y are out of range.");

                    while (--bytesPerPixel >= 0)
                    {
                        pixels[byteOffset++] = (byte)(value & 0xFF);
                        value >>= 8;
                    }
                    break;
                case PixelFormat.BlackWhite:
                    int index = x + y * Stride * 8;
                    if (x < 0) throw new ArgumentOutOfRangeException("x is less than zero", "x");
                    if (y < 0) throw new ArgumentOutOfRangeException("y is less than zero", "y");
                    if (x > Width) throw new ArgumentOutOfRangeException("x is greater than width", "x");
                    if (y > Height) throw new ArgumentOutOfRangeException("y is greater than height", "y");
                    if (index / 8 >= pixels.Length) throw new ArgumentException("x and y are out of range.");
                    if (value == 0)
                    {
                        pixels[index / 8] &= (byte)(~(128 >> (index % 8)));
                    }
                    else
                    {
                        pixels[index / 8] |= (byte)(128 >> (index % 8));
                    }
                    break;
            }

            ClearCache();
        }

        int IBitmapCanvas.GetPixel(int x, int y)
        {
            using (Lock())
            {
                return GetPixelCore(x, y);
            }
        }

        protected virtual int GetPixelCore(int x, int y)
        {
            int bytesPerPixel = 1;

            switch (PixelFormat)
            {
                case PixelFormat.Gray16:
                    bytesPerPixel = 2;
                    goto case PixelFormat.Indexed8;
                case PixelFormat.Indexed8:
                    int byteOffset = (Width * y + x) * bytesPerPixel;
                    if (x < 0) throw new ArgumentOutOfRangeException("x is less than zero", "x");
                    if (y < 0) throw new ArgumentOutOfRangeException("y is less than zero", "y");
                    if (x > Width) throw new ArgumentOutOfRangeException("x is greater than width", "x");
                    if (y > Height) throw new ArgumentOutOfRangeException("y is greater than height", "y");
                    if (byteOffset + bytesPerPixel > pixels.Length)
                        throw new ArgumentException("x and y are out of range.");

                    int value = 0;

                    while (--bytesPerPixel >= 0)
                    {
                        value <<= 8;
                        value |= pixels[byteOffset + bytesPerPixel];
                    }

                    return value;
                case PixelFormat.BlackWhite:
                    int index = x + y * Stride * 8;
                    return (pixels[index / 8] & (1 << (index % 8))) == 0 ? 0 : 1;
                default:
                    throw new InvalidOperationException("Failed to get pixel due to unknown pixel format.");
            }
        }

        public virtual void SetPixelBuffer(byte[] buffer)
        {
            using (Lock(true))
            {
                if (buffer == null)
                    throw new ArgumentNullException("buffer");
                if (buffer.Length != Stride * Height)
                    throw new ArgumentException("Bad buffer length.", "buffer");
                Contract.EndContractBlock();

                pixels = buffer;
            }
        }

        public virtual void CopyTo(IBitmapCanvas dest, int destX, int destY)
        {
            using (Lock())
            {
                for (int y = destY;y < dest.Height;y++)
                {
                    for (int x = destX;x < dest.Width;x++)
                    {
                        dest.SetPixel(x, y, ((IBitmapCanvas)this).GetPixel(x - destX, y - destY));
                    }
                }
            }
        }

        public void Clear(int fill)
        {
            using (Lock()) ClearCore(fill);
        }

        protected virtual void ClearCore(int fill)
        {
            if (fill == 0)
            {
                Array.Clear(pixels, 0, pixels.Length);
                ClearCache();
                return;
            }

            for (int y = 0;y < Height;y++)
            {
                for (int x = 0;x < Width;x++)
                {
                    ((IBitmapCanvas)this).SetPixel(x, y, fill);
                }
            }

            ClearCache();
        }

        private void ResizeBuffer(int offsetX, int offsetY, int oldWidth, int oldHeight, int newWidth, int newHeight)
        {
            int oldStride = CalculateStride(oldWidth, PixelFormat);
            int newStride = CalculateStride(newWidth, PixelFormat);
            var buffer = new byte[newStride * newHeight];
            int copyLength = newStride < oldStride ? newStride : oldStride;
            int h = newHeight < oldHeight ? newHeight : oldHeight;

            for (int y = 0;y < h;y++)
            {
                Array.Copy(pixels, oldStride * y, buffer, newStride * y, copyLength);
            }

            pixels = buffer;
        }

        protected override void OnPropertyChanging(string propertyName, object currentValue, object newValue)
        {
            if (propertyName == nameof(Width))
                ResizeBuffer(0, 0, (int)currentValue, Height, (int)newValue, Height);
            else if (propertyName == nameof(Height))
                ResizeBuffer(0, 0, Width, (int)currentValue, Width, (int)newValue);
            base.OnPropertyChanging(propertyName, currentValue, newValue);
        }
    }
}
