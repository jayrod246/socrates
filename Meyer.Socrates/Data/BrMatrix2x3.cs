namespace Meyer.Socrates.Data
{
    using System;

    public partial struct BrMatrix2x3
    {
        const int WIDTH = 2;
        const int HEIGHT = 3;

        public static readonly BrMatrix2x3 Identity = new BrMatrix2x3
        (
            1f, 0f,
            0f, 1f,
            0f, 0f
        );

        internal int m00;
        internal int m01;
        internal int m10;
        internal int m11;
        internal int m20;
        internal int m21;

        public BrScalar M00 { get => new BrScalar(m00); set => m00 = value.RawValue; }
        public BrScalar M01 { get => new BrScalar(m01); set => m01 = value.RawValue; }
        public BrScalar M10 { get => new BrScalar(m10); set => m10 = value.RawValue; }
        public BrScalar M11 { get => new BrScalar(m11); set => m11 = value.RawValue; }
        public BrScalar M20 { get => new BrScalar(m20); set => m20 = value.RawValue; }
        public BrScalar M21 { get => new BrScalar(m21); set => m21 = value.RawValue; }

        public BrMatrix2x3(BrScalar m00, BrScalar m01, BrScalar m10, BrScalar m11, BrScalar m20, BrScalar m21)
        {
            this.m00 = m00.RawValue;
            this.m01 = m01.RawValue;
            this.m10 = m10.RawValue;
            this.m11 = m11.RawValue;
            this.m20 = m20.RawValue;
            this.m21 = m21.RawValue;
        }

        public BrScalar this[int column, int row]
        {
            get
            {
                if (column < 0 || column >= WIDTH)
                    throw new ArgumentOutOfRangeException("column");
                if (row < 0 || row >= HEIGHT)
                    throw new ArgumentOutOfRangeException("row");

                return GetColumn(column)[row];
            }
            set
            {
                if (column < 0 || column >= WIDTH)
                    throw new ArgumentOutOfRangeException("column");
                if (row < 0 || row >= HEIGHT)
                    throw new ArgumentOutOfRangeException("row");

                typeof(BrMatrix3x4).GetField($"m{row}{column}").SetValue(this, value.RawValue);
            }
        }

        public BrVector3 GetColumn(int index)
        {
            switch (index)
            {
                case 0:
                    return new BrVector3(m00, m10, m20);
                case 1:
                    return new BrVector3(m01, m11, m21);
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        public BrVector2 GetRow(int index)
        {
            switch (index)
            {
                case 0:
                    return new BrVector2(m00, m01);
                case 1:
                    return new BrVector2(m10, m11);
                case 2:
                    return new BrVector2(m20, m21);
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        public static BrMatrix2x3 operator *(BrMatrix2x3 a, BrMatrix2x3 b)
        {
            return Multiply(a, b);
        }
    }
}
