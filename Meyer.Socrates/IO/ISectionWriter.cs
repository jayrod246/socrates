using Meyer.Socrates.Data.Sections;
using System;

namespace Meyer.Socrates.IO
{
    public interface ISectionWriter
    {
        void Write(VirtualSection o, IDataWriteContext c);
    }

    public interface ISectionWriter<in T>: ISectionWriter where T : VirtualSection
    {
        void Write(T o, IDataWriteContext c);
    }

    public static class SectionWriter
    {
        public static ISectionWriter Create(Action<VirtualSection, IDataWriteContext> write)
        {
            return new DelegateSectionWriter(write);
        }
        public static ISectionWriter Create<T>(Action<T, IDataWriteContext> write) where T : VirtualSection
        {
            return new DelegateSectionWriter<T>(write);
        }

        class DelegateSectionWriter: ISectionWriter
        {
            private Action<VirtualSection, IDataWriteContext> write;

            public DelegateSectionWriter(Action<VirtualSection, IDataWriteContext> write)
            {
                this.write = write;
            }

            public void Write(VirtualSection o, IDataWriteContext c)
            {
                write.Invoke(o, c);
            }
        }

        class DelegateSectionWriter<T>: ISectionWriter<T> where T : VirtualSection
        {
            private Action<T, IDataWriteContext> write;

            public DelegateSectionWriter(Action<T, IDataWriteContext> write)
            {
                this.write = write;
            }

            public void Write(T o, IDataWriteContext c)
            {
                write.Invoke(o, c);
            }

            public void Write(VirtualSection o, IDataWriteContext c)
            {
                write.Invoke((T)o, c);
            }
        }
    }
}
