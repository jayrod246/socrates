using Meyer.Socrates.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Meyer.Socrates.Test.Data
{
    [TestClass]
    public class BrMatrix3x4Tests
    {
        [TestMethod]
        public void BrMatrix3x4_ArithmeticTest()
        {
            // Multiplication
            var left = new BrMatrix3x4(3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36);
            var right = new BrMatrix3x4(2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24);
            var result = new BrMatrix3x4(180, 216, 252, 396, 486, 576, 612, 756, 900, 848, 1048, 1248);
            Assert.AreEqual(result, left * right);
        }
    }
}
