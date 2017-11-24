using Meyer.Socrates.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Meyer.Socrates.Test.Data
{
    [TestClass]
    public class BrScalarTests
    {
        [TestMethod]
        public void BrScalar_SingleEqualityTest()
        {
            Assert.AreEqual(1f, new BrScalar(1f).AsSingle());
        }

        [TestMethod]
        public void BrScalar_RawTest()
        {
            var value = BrScalar.FromRaw(65536);
            Assert.AreEqual(65536, value.RawValue);
            Assert.AreEqual(new BrScalar(1f), value);
        }

        [TestMethod]
        public void BrScalar_ArithmeticTest()
        {
            Assert.AreEqual(new BrScalar(100f), new BrScalar(75f) + new BrScalar(25f));
            Assert.AreEqual(new BrScalar(50f), new BrScalar(75f) - new BrScalar(25f));
            Assert.AreEqual(new BrScalar(20f), new BrScalar(5f) * new BrScalar(4f));
            Assert.AreEqual(new BrScalar(5f), new BrScalar(100f) / new BrScalar(20f));
        }
    }
}
