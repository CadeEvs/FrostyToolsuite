using System;
using System.IO;
using System.Security.Cryptography;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.Managers;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.IO;

public class BlockStream : DataStream
{
    private readonly Block<byte> m_block;

    public BlockStream(int inSize)
    {
        m_block = new Block<byte>(inSize);
        m_stream = m_block.ToStream();
    }

    public BlockStream(Block<byte> inBuffer)
    {
        m_block = inBuffer;
        m_stream = m_block.ToStream();
    }

    /// <summary>
    /// Loads whole file into memory and deobfuscates it if necessary.
    /// </summary>
    /// <param name="inPath">The path of the file</param>
    /// <param name="inShouldDeobfuscate">The boolean if the file needs to be deobfuscated</param>
    /// <returns>A <see cref="BlockStream"/> that has the file loaded.</returns>
    public static BlockStream FromFile(string inPath, bool inShouldDeobfuscate)
    {
        using (FileStream stream = new(inPath, FileMode.Open, FileAccess.Read))
        {
            BlockStream retVal;
            if (inShouldDeobfuscate)
            {
                Span<byte> header = stackalloc byte[0x22C];
                stream.ReadExactly(header);

                if (header[0] == 0x00 && header[1] == 0xD1 && header[2] == 0xCE &&
                    (header[3] == 0x00 || header[3] == 0x01 || header[3] == 0x03)) // version 0 is not used in fb3
                {
                    retVal = new BlockStream((int)(stream.Length - 0x22C));
                    stream.ReadExactly(retVal.m_block.ToSpan());
                    
                    // deobfuscate the data
                    IDeobfuscator? deobfuscator = FileSystemManager.CreateDeobfuscator();
                    deobfuscator?.Deobfuscate(header, retVal.m_block);
                    
                    return retVal;
                }
                stream.Position = 0;
            }

            retVal = new BlockStream((int)stream.Length);
            stream.ReadExactly(retVal.m_block.ToSpan());
            return retVal;
        }
    }

    /// <summary>
    /// Loads part of a file into memory.
    /// </summary>
    /// <param name="inPath">The path of the file.</param>
    /// <param name="inOffset">The offset of the data to load.</param>
    /// <param name="inSize">The size of the data to load</param>
    /// <returns>A <see cref="BlockStream"/> that has the data loaded.</returns>
    public static BlockStream FromFile(string inPath, long inOffset, int inSize)
    {
        using (FileStream stream = new(inPath, FileMode.Open, FileAccess.Read))
        {
            stream.Position = inOffset;

            BlockStream retVal = new(inSize);
            
            stream.ReadExactly(retVal.m_block.ToSpan());
            return retVal;
        }
    }

    /// <summary>
    /// <see cref="Aes"/> decrypt this <see cref="BlockStream"/>.
    /// </summary>
    /// <param name="inKey">The key to use for the decryption.</param>
    /// <param name="inPaddingMode">The <see cref="PaddingMode"/> to use for the decryption.</param>
    public void Decrypt(byte[] inKey, PaddingMode inPaddingMode)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = inKey;
            long curPos = Position;
            m_block.Shift((int)Position);
            aes.DecryptCbc(m_block.ToSpan(), inKey, m_block.ToSpan(), inPaddingMode);
            m_block.ResetShift();
            Position = curPos;
        }
    }

    /// <summary>
    /// <see cref="Aes"/> decrypt part of this <see cref="BlockStream"/>.
    /// </summary>
    /// <param name="inKey">The key to use for the decryption.</param>
    /// <param name="inSize">The size of the data to decrypt.</param>
    /// <param name="inPaddingMode">The <see cref="PaddingMode"/> to use for the decryption.</param>
    public void Decrypt(byte[] inKey, int inSize, PaddingMode inPaddingMode)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = inKey;
            long curPos = Position;
            m_block.Shift((int)Position);
            aes.DecryptCbc(m_block.Slice(0, inSize).ToSpan(), inKey, m_block.Slice(0, inSize).ToSpan(), inPaddingMode);
            m_block.ResetShift();
            Position = curPos;
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        m_block.Dispose();
        GC.SuppressFinalize(this);
    }
}