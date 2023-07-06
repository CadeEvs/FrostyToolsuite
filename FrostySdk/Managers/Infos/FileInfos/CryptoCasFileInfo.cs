using System;
using System.IO;
using System.Security.Cryptography;
using Frosty.Sdk.IO;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.Managers.Infos.FileInfos;

public class CryptoCasFileInfo : CasFileInfo
{
    private uint m_encryptedSize => m_size + m_size % 16;
    private string m_keyId = string.Empty;
    private byte[] m_checksum = new byte[32];

    internal CryptoCasFileInfo()
    {
    }
    
    public CryptoCasFileInfo(CasFileIdentifier inCasFileIdentifier, uint inOffset, uint inSize, uint inLogicalOffset, string inKeyId, byte[] inChecksum)
        : base(inCasFileIdentifier, inOffset, inSize, inLogicalOffset)
    {
        m_keyId = inKeyId;
        m_checksum = inChecksum;
    }

    public Block<byte> GetNonDecryptedRawData()
    {
        using (FileStream stream = new(FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(m_casFileIdentifier)), FileMode.Open, FileAccess.Read))
        {
            stream.Position = m_offset;

            Block<byte> retVal = new((int)m_encryptedSize);
            
            stream.ReadExactly(retVal.ToSpan());
            return retVal;
        }
    }
    
    public override Block<byte> GetRawData()
    {
        using (FileStream stream = new(FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(m_casFileIdentifier)), FileMode.Open, FileAccess.Read))
        {
            stream.Position = m_offset;

            Block<byte> retVal = new((int)m_encryptedSize);
            
            stream.ReadExactly(retVal.ToSpan());

            using (Aes aes = Aes.Create())
            {
                byte[] key = KeyManager.GetKey(m_keyId);
                aes.Key = key;
                
                aes.DecryptCbc(retVal.ToSpan(), key, retVal.ToSpan());
            }

            retVal.Resize((int)m_size);
            
            return retVal;
        }
    }

    public override Block<byte> GetData(int originalSize)
    {
        using (BlockStream stream = BlockStream.FromFile(FileSystemManager.ResolvePath(FileSystemManager.GetFilePath(m_casFileIdentifier)), m_offset, (int)m_encryptedSize))
        {
            stream.Decrypt(KeyManager.GetKey(m_keyId), PaddingMode.PKCS7);
            return Cas.DecompressData(stream, originalSize);
        }
    }
    
    protected override void DeserializeInternal(DataStream stream)
    {
        base.DeserializeInternal(stream);
        m_keyId = stream.ReadFixedSizedString(8);
        stream.ReadExactly(m_checksum);
    }

    protected override void SerializeInternal(DataStream stream)
    {
        base.SerializeInternal(stream);
        stream.WriteFixedSizedString(m_keyId, 8);
        stream.Write(m_checksum);
    }

    public bool Equals(CryptoCasFileInfo b)
    {
        return base.Equals(b) &&
               m_keyId == b.m_keyId &&
               m_checksum == b.m_checksum;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is CryptoCasFileInfo b)
        {
            return Equals(b);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), m_keyId, m_checksum);
    }
}