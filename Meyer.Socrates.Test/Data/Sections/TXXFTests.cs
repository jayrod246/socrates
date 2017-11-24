using Meyer.Socrates.Data;
using Meyer.Socrates.Data.Sections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Meyer.Socrates.Test.Data.Sections
{
    [TestClass]
    public class TXXFTests
    {
        [TestMethod]
        public void TXXF_PropertiesTest()
        {
            const int x = 45;
            const int y = 60;
            const int scalex = 2;
            const int scaley = 4;

            var section = new TXXF();

            Assert.AreEqual(1, section.TransformMatrix[0, 0]);
            Assert.AreEqual(1, section.TransformMatrix[1, 1]);
            Assert.AreEqual(0, section.TransformMatrix[0, 2]);
            Assert.AreEqual(0, section.TransformMatrix[1, 2]);
            section.Position = new BrVector2(x, y);
            Assert.AreEqual(x, section.TransformMatrix[0, 2]);
            Assert.AreEqual(y, section.TransformMatrix[1, 2]);
            section.Scale = new BrVector2(scalex, scaley);
            Assert.AreEqual(scalex, section.TransformMatrix[0, 0]);
            Assert.AreEqual(scaley, section.TransformMatrix[1, 1]);
            Assert.AreEqual(x, section.TransformMatrix[0, 2]);
            Assert.AreEqual(y, section.TransformMatrix[1, 2]);

            //const double angle = 90.0;

            //var angle_radians = (Math.PI / 180.0) * angle;

            //var rotation = new BrMatrix2x2((BrScalar)Math.Cos(angle_radians), (BrScalar)(-Math.Sin(angle_radians)), (BrScalar)Math.Sin(angle_radians), (BrScalar)Math.Cos(angle_radians));
            //section.Rotation = rotation;
        }
    }
}
