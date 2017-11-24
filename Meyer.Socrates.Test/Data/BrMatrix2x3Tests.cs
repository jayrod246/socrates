using Meyer.Socrates.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Meyer.Socrates.Test.Data
{
    [TestClass]
    public class BrMatrix2x3Tests
    {
        [TestMethod]
        public void BrMatrix2x3_ArithmeticTest()
        {
            // Multiplication
            var left = new BrMatrix2x3(3, 6, 9, 12, 15, 18);
            var right = new BrMatrix2x3(2, 4, 6, 8, 10, 12);
            var result = new BrMatrix2x3(42, 60, 90, 132, 148, 216);
            Assert.AreEqual(result, left * right);
        }

        [TestMethod]
        public void BrMatrix2x3_TRSTest()
        {
            var t = new BrVector2(45, 95);
            var r = new BrAngle(90);
            var s = new BrVector2(0.5f, 0.67f);
            var mat = BrMatrix2x3.FromTRS(t, r, s);
            mat.TRS(out var mt, out var mr, out var ms);
            Assert.AreEqual(t, mt);
            Assert.IsTrue(Math.Abs(r -mr) < 1);
            Assert.IsTrue(Math.Abs(BrVector.GetMagnitude(s) - BrVector.GetMagnitude(ms)) < 0.1f);
        }
    }
}
