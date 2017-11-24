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

        public BrScalar M00 { get => BrScalar.FromRaw(m00); set => m00 = value.RawValue; }
        public BrScalar M01 { get => BrScalar.FromRaw(m01); set => m01 = value.RawValue; }
        public BrScalar M02 { get => BrScalar.FromRaw(m02); set => m02 = value.RawValue; }
        public BrScalar M10 { get => BrScalar.FromRaw(m10); set => m10 = value.RawValue; }
        public BrScalar M11 { get => BrScalar.FromRaw(m11); set => m11 = value.RawValue; }
        public BrScalar M12 { get => BrScalar.FromRaw(m12); set => m12 = value.RawValue; }
        public BrScalar M20 { get => BrScalar.FromRaw(m20); set => m20 = value.RawValue; }
        public BrScalar M21 { get => BrScalar.FromRaw(m21); set => m21 = value.RawValue; }
        public BrScalar M22 { get => BrScalar.FromRaw(m22); set => m22 = value.RawValue; }
        public BrScalar M30 { get => BrScalar.FromRaw(m30); set => m30 = value.RawValue; }
        public BrScalar M31 { get => BrScalar.FromRaw(m31); set => m31 = value.RawValue; }
        public BrScalar M32 { get => BrScalar.FromRaw(m32); set => m32 = value.RawValue; }

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

                var vec = GetColumn(column);
                vec[row] = value;
                SetColumn(column, vec);
            }
        }

        public void SetColumn(int index, BrVector4 value)
        {
            switch (index)
            {
                case 0:
                    m00 = value.v0;
                    m10 = value.v1;
                    m20 = value.v2;
                    m30 = value.v3;
                    break;
                case 1:
                    m01 = value.v0;
                    m11 = value.v1;
                    m21 = value.v2;
                    m31 = value.v3;
                    break;
                case 2:
                    m02 = value.v0;
                    m12 = value.v1;
                    m22 = value.v2;
                    m32 = value.v3;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        public void SetRow(int index, BrVector3 value)
        {
            switch (index)
            {
                case 0:
                    m00 = value.v0;
                    m01 = value.v1;
                    m02 = value.v2;
                    break;
                case 1:
                    m10 = value.v0;
                    m11 = value.v1;
                    m12 = value.v2;
                    break;
                case 2:
                    m20 = value.v0;
                    m21 = value.v1;
                    m22 = value.v2;
                    break;
                case 3:
                    m30 = value.v0;
                    m31 = value.v1;
                    m32 = value.v2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        public BrVector4 GetColumn(int index)
        {
            switch (index)
            {
                case 0:
                    return BrVector4.FromRaw(m00, m10, m20, m30);
                case 1:
                    return BrVector4.FromRaw(m01, m11, m21, m31);
                case 2:
                    return BrVector4.FromRaw(m02, m12, m22, m32);
                default:
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        public BrVector3 GetRow(int index)
        {
            switch (index)
            {
                case 0:
                    return BrVector3.FromRaw(m00, m01, m02);
                case 1:
                    return BrVector3.FromRaw(m10, m11, m12);
                case 2:
                    return BrVector3.FromRaw(m20, m21, m22);
                case 3:
                    return BrVector3.FromRaw(m30, m31, m32);
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
