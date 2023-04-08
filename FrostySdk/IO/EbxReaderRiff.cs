using System.IO;

namespace Frosty.Sdk.IO;

public class EbxReaderRiff : EbxReader
{
    public EbxReaderRiff(Stream inStream)
        : base(inStream)
    {
    }
}