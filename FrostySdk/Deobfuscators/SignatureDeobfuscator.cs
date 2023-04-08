using System.Drawing;
using System.IO;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.Deobfuscators;

public class SignatureDeobfuscator : IDeobfuscator
{
    public Stream? Initialize(DataStream stream)
    {
        // | magic | unused |      signature      |      unused      |
        uint magic = stream.ReadUInt32();
        if (magic != 0x00CED100 && magic != 0x01CED100 && magic != 0x03CED100)
        {
            stream.Position = 0;
            return null;
        }
        
        stream.Position = 0x22C;

        byte[] data = stream.ReadBytes((int)(stream.Length - 0x22C));
        
        return new MemoryStream(data);
    }
}