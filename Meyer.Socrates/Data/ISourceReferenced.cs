using Meyer.Socrates.IO;

namespace Meyer.Socrates.Data
{
    public interface ISourceReferenced
    {
        SourceReference SourceReference { get; set; }
        Ms3dmmFile Container { get; }
    }
}