using System.IO;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.Deobfuscators;

public class LegacyDeobfuscator : IDeobfuscator
{
    private byte[]? m_key;

    public Stream? Initialize(DataStream stream)
    {
        uint magic = stream.ReadUInt32();
        if (magic != 0x00CED100 && magic != 0x01CED100 && magic != 0x03CED100)
        {
            stream.Position = 0;
            return null;
        }
        
        stream.Position = 0x22C;

        byte[] data = stream.ReadBytes((int)(stream.Length - 0x22C));
        
        if (magic == 0x01CED100)
        {
            stream.Position = 0x128;

            m_key = stream.ReadBytes(0x101);

            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= (byte)(0x7B ^ m_key[i % 0x101]);
            }
        }
        
        return new MemoryStream(data);
    }
}