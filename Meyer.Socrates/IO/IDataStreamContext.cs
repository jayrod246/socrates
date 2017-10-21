namespace Meyer.Socrates.IO
{
    public interface IDataStreamContext
    {
        long Position { get; set; }
        long Length { get; }
    }
}
