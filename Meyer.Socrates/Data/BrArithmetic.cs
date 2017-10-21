using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meyer.Socrates.Data
{
    #region Matrices
    // Matrix3x4
    public partial struct BrMatrix3x4
    {
        public static BrMatrix3x4 Multiply(BrMatrix3x4 left, BrMatrix3x4 right)
        {
            return new BrMatrix3x4(left.M00 * right.M00 + left.M01 * right.M10 + left.M02 * right.M20, left.M00 * right.M01 + left.M01 * right.M11 + left.M02 * right.M21, left.M00 * right.M02 + left.M01 * right.M12 + left.M02 * right.M22,
                                left.M10 * right.M00 + left.M11 * right.M10 + left.M12 * right.M20, left.M10 * right.M01 + left.M11 * right.M11 + left.M12 * right.M21, left.M10 * right.M02 + left.M11 * right.M12 + left.M12 * right.M22,
                                left.M20 * right.M00 + left.M21 * right.M10 + left.M22 * right.M20, left.M20 * right.M01 + left.M21 * right.M11 + left.M22 * right.M21, left.M20 * right.M02 + left.M21 * right.M12 + left.M22 * right.M22,
                                left.M30 * right.M00 + left.M31 * right.M10 + left.M32 * right.M20 + right.M30, left.M30 * right.M01 + left.M31 * right.M11 + left.M32 * right.M21 + right.M31, left.M30 * right.M02 + left.M31 * right.M12 + left.M32 * right.M22 + right.M32);
        }
    }
    #endregion

    #region Scalar
    public partial struct BrScalar
    {
        public static BrScalar Multiply(BrScalar a, BrScalar b)
        {
            return new BrScalar(a.AsSingle() * b.AsSingle());
        }
    }
    #endregion
}
