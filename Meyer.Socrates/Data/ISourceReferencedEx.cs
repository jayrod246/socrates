using Meyer.Socrates.IO;

namespace Meyer.Socrates.Data
{
    public interface ISourceReferencedEx
    {
        SourceReferenceEx SourceReference { get; set; }
        Ms3dmmFile Container { get; }
    }
}