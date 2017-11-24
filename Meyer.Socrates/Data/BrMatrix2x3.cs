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

        public void TRS(out BrVector2 translation, out BrAngle rotation, out BrVector2 scale)
        {
            var matrix = this;
            translation = matrix.GetRow(2);
            matrix.SetRow(2, new BrVector2());
            scale = new BrVector2(BrVector.GetMagnitude(matrix.GetRow(0)), BrVector.GetMagnitude(matrix.GetRow(1)));
            for (int row = 0;row < 2;row++)
            {
                for (int col = 0;col < 2;col++)
                {
                    var sc = scale[row];
                    if (sc == 0)
                        matrix[col, row] = 0f;
                    else matrix[col, row] /= sc;
                }
            }

            rotation = BrAngle.FromRadians((-(float)Math.Asin(matrix[1, 0]) +
                                             (float)Math.Acos(matrix[1, 1]) +
                                             (float)Math.Acos(matrix[0, 0]) +
                                             (float)Math.Asin(matrix[0, 1])) / 4f);

        }

        public static BrMatrix2x3 FromTRS(BrVector2 translation, BrAngle rotation, BrVector2 scale)
        {
            var mt = new BrMatrix2x3(
                1, 0,
                0, 1,
                translation.X, translation.Y);

            var c = (BrScalar)Math.Cos(rotation.AsSingle(true));
            var s = (BrScalar)Math.Sin(rotation.AsSingle(true));
            var mr = new BrMatrix2x3(
                c, -s,
                s, c,
                0, 0);

            var ms = new BrMatrix2x3(
                scale.X, 0,
                0, scale.Y,
                0, 0);

            return ms * mr * mt;
        }

        public static BrMatrix2x3 operator *(BrMatrix2x3 a, BrMatrix2x3 b)
        {
            return Multiply(a, b);
        }
    }
}
