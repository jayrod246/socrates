using Meyer.Socrates.Data.Sections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meyer.Socrates.IO;

namespace Meyer.Socrates.Test.Data.Sections
{
    [TestClass]
    public class IndexableSectionTests
    {
        [TestMethod]
        public void IndexableSection_ReadTest()
        {
            var data = new byte[] { 1 };
            var sect = new MyIndexableSection() { Data = data };
            var oldItem = sect[0];
            Assert.AreSame(sect, oldItem.container);
            sect.Data = Array.Empty<byte>();
            Assert.ThrowsException<InvalidOperationException>(() => sect[0]);
            Assert.AreSame(sect, oldItem.container);
            sect.Data = data;
            Assert.AreSame(null, oldItem.container);
            Assert.AreNotSame(oldItem, sect[0]);
            Assert.AreSame(sect, sect[0].container);
        }

        class MyIndexableSection: IndexableSection<MyIndexableSection.MyIndexableItem>
        {
            public class MyIndexableItem
            {
                public object container;
            }

            protected override void Read(IDataReadContext c)
            {
                if (c.Length > 0)
                    Add(new MyIndexableItem());
                else
                    throw new InvalidOperationException();
            }

            protected override void Write(IDataWriteContext c)
            {
                c.Write(1);
            }

            protected override void InsertItem(int index, MyIndexableItem item)
            {
                base.InsertItem(index, item);
                item.container = this;
            }

            protected override void RemoveItem(int index, MyIndexableItem item)
            {
                base.RemoveItem(index, item);
                item.container = null;
            }
        }
    }
}
