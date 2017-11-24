using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Meyer.Socrates.Data;

namespace Meyer.Socrates.Test.Data
{
    [TestClass]
    public class BrAngleTests
    {
        [TestMethod]
        public void BrAngle_ValueTest()
        {
            for (int i = -360;i < 720;i++)
            {
                const double eps = 0.01;
                var angle = BrAngle.FromDegrees(i);
                var expected = i % 360f;
                expected = expected < 0 ? 360f + expected : expected;
                Assert.IsTrue(Math.Abs(expected - angle.Value) <= eps, $"Expected {expected} and got {angle.Value}");
            }
        }
    }
}
