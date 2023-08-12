using System.IO;
using System.Security.Cryptography;
using Frosty.Sdk.IO;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.Managers.Infos.FileInfos;

public class CasCryptoResourceInfo : CasResourceInfo
{
    private readonly string m_keyId;
    
    public CasCryptoResourceInfo(CasFileIdentifier inCasFileIdentifier, uint inOffset, uint inSize, uint inLogicalOffset, string inKeyId)
        : base(inCasFileIdentifier, inOffset, inSize, inLogicalOffset)
    {
        m_keyId = inKeyId;
    }

    public CasCryptoResourceInfo(bool inIsPatch, int inInstallChunkIndex, int inCasIndex, uint inOffset, uint inSize, uint inLogicalOffset, string inKeyId)
        : base(inIsPatch, inInstallChunkIndex, inCasIndex, inOffset, inSize, inLogicalOffset)
    {
        m_keyId = inKeyId;
    }

    public override Block<byte> GetRawData()
    {
        using (FileStream stream = new(GetPath(), FileMode.Open, FileAccess.Read))
        {
            stream.Position = GetOffset();

            // we need to align the size to 16
            int size = (int)GetSize();
            size += size & 15;
            Block<byte> retVal = new(size);
            
            stream.ReadExactly(retVal);
            return retVal;
        }
    }

    public override Block<byte> GetData(int inOriginalSize)
    {
        // we need to align the size to 16
        int size = (int)GetSize();
        size += size & 15;
        using (BlockStream stream = BlockStream.FromFile(GetPath(), GetOffset(), size))
        {
            stream.Decrypt(KeyManager.GetKey(m_keyId), PaddingMode.PKCS7);
            return Cas.DecompressData(stream, inOriginalSize);
        }
    }

    internal static void SerializeInternal(DataStream stream, CasCryptoResourceInfo info)
    {
        CasResourceInfo.SerializeInternal(stream, info);
        stream.WriteNullTerminatedString(info.m_keyId);
    }

    internal static CasCryptoResourceInfo DeserializeInternal(DataStream stream)
    {
        CasFileIdentifier file = CasFileIdentifier.FromFileIdentifier(stream.ReadUInt32());
        uint offset = stream.ReadUInt32();
        uint size = stream.ReadUInt32();
        uint logicalOffset = stream.ReadUInt32();
        string keyId = stream.ReadNullTerminatedString();

        return new CasCryptoResourceInfo(file, offset, size, logicalOffset, keyId);
    }
}