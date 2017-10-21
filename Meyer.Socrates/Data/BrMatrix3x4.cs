namespace Meyer.Socrates.Data
{
    using System;

    public partial struct BrMatrix3x4
    {
        const int WIDTH = 3;
        const int HEIGHT = 4;

        public static readonly BrMatrix3x4 Identity = new BrMatrix3x4
        (
            1f, 0f, 0f,
            0f, 1f, 0f,
            0f, 0f, 1f,
            0f, 0f, 0f
        );

        internal int m00;
        internal int m01;
        internal int m02;
        internal int m10;
        internal int m11;
        internal int m12;
        internal int m20;
        internal int m21;
        internal int m22;
        internal int m30;
        internal int m31;
        internal int m32;

        public BrScalar M00 { get => new BrScalar(m00); set => m00 = value.RawValue; }
        public BrScalar M01 { get => new BrScalar(m01); set => m01 = value.RawValue; }
        public BrScalar M02 { get => new BrScalar(m02); set => m02 = value.RawValue; }
        public BrScalar M10 { get => new BrScalar(m10); set => m10 = value.RawValue; }
        public BrScalar M11 { get => new BrScalar(m11); set => m11 = value.RawValue; }
        public BrScalar M12 { get => new BrScalar(m12); set => m12 = value.RawValue; }
        public BrScalar M20 { get => new BrScalar(m20); set => m20 = value.RawValue; }
        public BrScalar M21 { get => new BrScalar(m21); set => m21 = value.RawValue; }
        public BrScalar M22 { get => new BrScalar(m22); set => m22 = value.RawValue; }
        public BrScalar M30 { get => new BrScalar(m30); set => m30 = value.RawValue; }
        public BrScalar M31 { get => new BrScalar(m31); set => m31 = value.RawValue; }
        public BrScalar M32 { get => new BrScalar(m32); set => m32 = value.RawValue; }

        public BrMatrix3x4(BrScalar m00, BrScalar m01, BrScalar m02, BrScalar m10, BrScalar m11, BrScalar m12, BrScalar m20, BrScalar m21, BrScalar m22, BrScalar m30, BrScalar m31, BrScalar m32)
        {
            this.m00 = m00.RawValue;
            this.m01 = m01.RawValue;
            this.m02 = m02.RawValue;
            this.m10 = m10.RawValue;
            this.m11 = m11.RawValue;
            this.m12 = m12.RawValue;
            this.m20 = m20.RawValue;
            this.m21 = m21.RawValue;
            this.m22 = m22.RawValue;
            this.m30 = m30.RawValue;
            this.m31 = m31.RawValue;
            this.m32 = m32.RawValue;
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

        public BrVector4 GetColumn(int index)
        {
            switch (index)
            {
                case 0:
                    return new BrVector4(m00, m10, m20, m30);
                case 1:
                    return new BrVector4(m01, m11, m21, m31);
                case 2:
                    return new BrVector4(m02, m12, m22, m32);
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        public BrVector3 GetRow(int index)
        {
            switch (index)
            {
                case 0:
                    return new BrVector3(m00, m01, m02);
                case 1:
                    return new BrVector3(m10, m11, m12);
                case 2:
                    return new BrVector3(m20, m21, m22);
                case 3:
                    return new BrVector3(m30, m31, m32);
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        public static BrMatrix3x4 operator *(BrMatrix3x4 a, BrMatrix3x4 b)
        {
            return Multiply(a, b);
        }
    }
}
