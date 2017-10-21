namespace Meyer.Socrates.IO
{
    public interface IDataReadContext: IDataStreamContext
    {
        T Read<T>() where T : struct;
        T[] ReadArray<T>(int count) where T : struct;
        void ReadArray<T>(T[] arr, int count) where T : struct;
        void ReadArray<T>(T[] arr, int offset, int count) where T : struct;
        void Assert<T>(T value) where T : struct;
        T AssertAny<T>(params T[] value) where T : struct;
        void AssertArray<T>(T[] arr) where T : struct;
    }
}
