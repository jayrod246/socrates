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

        public BrScalar M00 { get => BrScalar.FromRaw(m00); set => m00 = value.RawValue; }
        public BrScalar M01 { get => BrScalar.FromRaw(m01); set => m01 = value.RawValue; }
        public BrScalar M10 { get => BrScalar.FromRaw(m10); set => m10 = value.RawValue; }
        public BrScalar M11 { get => BrScalar.FromRaw(m11); set => m11 = value.RawValue; }
        public BrScalar M20 { get => BrScalar.FromRaw(m20); set => m20 = value.RawValue; }
        public BrScalar M21 { get => BrScalar.FromRaw(m21); set => m21 = value.RawValue; }

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

                var vec = GetColumn(column);
                vec[row] = value;
                SetColumn(column, vec);
            }
        }

        public void SetColumn(int index, BrVector3 value)
        {
            switch (index)
            {
                case 0:
                    m00 = value.v0;
                    m10 = value.v1;
                    m20 = value.v2;
                    break;
                case 1:
                    m01 = value.v0;
                    m11 = value.v1;
                    m21 = value.v2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        public void SetRow(int index, BrVector2 value)
        {
            switch (index)
            {
                case 0:
                    m00 = value.v0;
                    m01 = value.v1;
                    break;
                case 1:
                    m10 = value.v0;
                    m11 = value.v1;
                    break;
                case 2:
                    m20 = value.v0;
                    m21 = value.v1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        public BrVector3 GetColumn(int index)
        {
            switch (index)
            {
                case 0:
                    return BrVector3.FromRaw(m00, m10, m20);
                case 1:
                    return BrVector3.FromRaw(m01, m11, m21);
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        public BrVector2 GetRow(int index)
        {
            switch (index)
            {
                case 0:
                    return BrVector2.FromRaw(m00, m01);
                case 1:
                    return BrVector2.FromRaw(m10, m11);
                case 2:
                    return BrVector2.FromRaw(m20, m21);
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
