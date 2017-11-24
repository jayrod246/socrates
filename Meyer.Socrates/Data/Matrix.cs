using System;
using System.Linq;

namespace Meyer.Socrates.Data
{
    public class Matrix: IEquatable<Matrix>, IEquatable<BrMatrix2x3>, IEquatable<BrMatrix3x4>
    {
        public BrScalar this[int column, int row]
        {
            get
            {
                if (column < 0 || column >= width)
                    throw new ArgumentOutOfRangeException("column");
                if (row < 0 || row >= height)
                    throw new ArgumentOutOfRangeException("row");

                return GetColumn(column)[row];
            }
            set
            {
                if (column < 0 || column >= width)
                    throw new ArgumentOutOfRangeException("column");
                if (row < 0 || row >= height)
                    throw new ArgumentOutOfRangeException("row");

                flat[row * width + column] = value;
            }
        }

        private Matrix(bool undefined)
        {
            flat = Array.Empty<BrScalar>();
            width = 0;
            height = 0;
        }

        public Matrix(int columns, int rows)
        {
            if (columns <= 0)
                throw new ArgumentOutOfRangeException("columns");
            if (rows <= 0)
                throw new ArgumentOutOfRangeException("rows");
            width = columns;
            height = rows;
            flat = new BrScalar[columns * rows];
        }

        public Matrix(int columns, int rows, params BrScalar[] values)
        {
            if (columns <= 0)
                throw new ArgumentOutOfRangeException("columns");
            if (rows <= 0)
                throw new ArgumentOutOfRangeException("rows");
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length < rows * columns)
                throw new ArgumentException("Not enough values provided to make the matrix.", "values");
            width = columns;
            height = rows;
            flat = values;
        }

        public bool Equals(Matrix other)
        {
            return other != null && flat.SequenceEqual(other.flat);
        }

        public override bool Equals(object obj)
        {
            if (obj is Matrix other)
                return Equals(other);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return width ^ height;
        }

        public static Matrix Multiply(Matrix left, Matrix right)
        {
            if (left.width != right.height) throw new InvalidOperationException("The product of two matrices was undefined.");

            var matrix = new Matrix(right.width, left.height);

            var columnCache = Enumerable.Range(0, right.width).Select(x => right.GetColumn(x)).ToArray();

            for (int i = 0;i < left.height;i++) // For each row in left..
            {
                var a = left.GetRow(i);
                for (int j = 0;j < right.width;j++) // For each column in right..
                {
                    var b = columnCache[j];

                    matrix[j, i] = Enumerable.Range(0, left.width).Aggregate<int, BrScalar>(0, (s, k) => s + a[k] * b[k]);
                }
            }

            return matrix;
        }

        public void SetColumn(int index, params BrScalar[] values)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("Index must be greater than or equal to zero.", "index");
            if (index >= width)
                throw new ArgumentOutOfRangeException("Index exceeds the width of the matrix.", "index");
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length < height) throw new ArgumentException("Not enough values to set the column of BrMatrix", "values");

            for (int i = 0;i < height;i++)
                flat[i * width + index] = values[i];
        }

        public void SetRow(int index, params BrScalar[] values)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("Index must be greater than or equal to zero.", "index");
            if (index >= height)
                throw new ArgumentOutOfRangeException("Index exceeds the height of the matrix.", "index");
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length < width) throw new ArgumentException("Not enough values to set the row of BrMatrix", "values");

            for (int i = 0;i < width;i++)
                flat[index * width + i] = values[i];
        }

        public BrScalar[] GetColumn(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("Index must be greater than or equal to zero.", "index");
            if (index >= width)
                throw new ArgumentOutOfRangeException("Index exceeds the width of the matrix.", "index");

            var result = new BrScalar[height];

            for (int i = 0;i < height;i++)
                result[i] = flat[i * width + index];
            return result;
        }

        public BrScalar[] GetRow(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("Index must be greater than or equal to zero.", "index");
            if (index >= height)
                throw new ArgumentOutOfRangeException("Index exceeds the height of the matrix.", "index");

            var result = new BrScalar[width];

            for (int i = 0;i < width;i++)
                result[i] = flat[index * width + i];

            return result;
        }

        #region Conversions

        public static implicit operator Matrix(BrMatrix2x3 matrix)
        {
            return new Matrix(3, 3, matrix.M00, matrix.M01, 0f, matrix.M10, matrix.M11, 0f, matrix.M20, matrix.M21, 1f);
        }

        public static implicit operator Matrix(BrMatrix3x4 matrix)
        {
            return new Matrix(4, 4, matrix.M00, matrix.M01, matrix.M02, 0f, matrix.M10, matrix.M11, matrix.M12, 0f, matrix.M20, matrix.M21, matrix.M22, 0f, matrix.M30, matrix.M31, matrix.M32, 1f);
        }

        #endregion

        public static Matrix operator *(Matrix left, Matrix right)
        {
            return Multiply(left, right);
        }

        internal BrScalar[] flat;
        internal int width;
        internal int height;

        public bool Equals(BrMatrix2x3 other)
        {
            return Equals((Matrix)other);
        }

        public bool Equals(BrMatrix3x4 other)
        {
            return Equals((Matrix)other);
        }
    }
}
