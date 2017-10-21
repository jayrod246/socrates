using Meyer.Socrates.Data.Sections;
using System;

namespace Meyer.Socrates.IO
{
    public interface ISectionReader
    {
        void Read(VirtualSection o, IDataReadContext c);
    }

    public interface ISectionReader<in T>: ISectionReader where T : VirtualSection
    {
        void Read(T o, IDataReadContext c);
    }

    public static class SectionReader
    {
        public static ISectionReader Create(Action<VirtualSection, IDataReadContext> read)
        {
            return new DelegateSectionReader(read);
        }
        public static ISectionReader Create<T>(Action<T, IDataReadContext> read) where T : VirtualSection
        {
            return new DelegateSectionReader<T>(read);
        }

        class DelegateSectionReader: ISectionReader
        {
            private Action<VirtualSection, IDataReadContext> read;

            public DelegateSectionReader(Action<VirtualSection, IDataReadContext> read)
            {
                this.read = read;
            }

            public void Read(VirtualSection o, IDataReadContext c)
            {
                read.Invoke(o, c);
            }
        }

        class DelegateSectionReader<T>: ISectionReader<T> where T : VirtualSection
        {
            private Action<T, IDataReadContext> read;

            public DelegateSectionReader(Action<T, IDataReadContext> read)
            {
                this.read = read;
            }

            public void Read(T o, IDataReadContext c)
            {
                read.Invoke(o, c);
            }

            public void Read(VirtualSection o, IDataReadContext c)
            {
                read.Invoke((T)o, c);
            }
        }
    }
}
