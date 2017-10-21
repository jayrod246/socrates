namespace Meyer.Socrates.IO
{
    public interface IDataWriteContext: IDataStreamContext
    {
        void Write<T>(T value) where T : struct;
        void WriteArray<T>(T[] arr) where T : struct;
        void WriteArray<T>(T[] arr, int count) where T : struct;
        void WriteArray<T>(T[] arr, int offset, int count) where T : struct;
    }
}
