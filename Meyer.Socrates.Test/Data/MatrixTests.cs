using Meyer.Socrates.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Meyer.Socrates.Test.Data
{
    [TestClass]
    public class MatrixTests
    {
        [TestMethod]
        public void Matrix_ArithmeticTest()
        {
            Matrix left;
            Matrix right;
            Matrix result;

            // Multiplication
            left = new Matrix(4, 4, 3, 6, 9, 0, 12, 15, 18, 0, 21, 24, 27, 0, 30, 33, 36, 1);
            right = new Matrix(4, 4, 2, 4, 6, 0, 8, 10, 12, 0, 14, 16, 18, 0, 20, 22, 24, 1);
            result = new Matrix(4, 4, 180, 216, 252, 0, 396, 486, 576, 0, 612, 756, 900, 0, 848, 1048, 1248, 1);
            Assert.AreEqual(result, left * right);

            // Product Undefined
            left = new Matrix(3, 4, 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36);
            right = new Matrix(3, 4, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24);
            Assert.ThrowsException<InvalidOperationException>(() => left * right);
        }
    }
}
