using System;
using System.IO;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers.Infos.FileInfos;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.Interfaces;

public interface IFileInfo
{
    public bool IsComplete();

    public long GetSize();
    
    public Block<byte> GetRawData();
    
    public Block<byte> GetData(int originalSize = 0);

    protected void SerializeInternal(DataStream stream);
    protected void DeserializeInternal(DataStream stream);

    public static void Serialize(DataStream stream, IFileInfo fileInfo)
    {
        switch (fileInfo)
        {
            case CryptoCasFileInfo:
                stream.WriteByte(1);
                break;
            case CasFileInfo:
                stream.WriteByte(0);
                break;
            case PatchFileInfo:
                stream.WriteByte(2);
                break;
            case PathFileInfo:
                stream.WriteByte(3);
                break;
            default:
                throw new NotImplementedException();
        }
        fileInfo.SerializeInternal(stream);
    }
    public static IFileInfo Deserialize(DataStream stream)
    {
        IFileInfo fileInfo;
        byte type = stream.ReadByte();

        switch (type)
        {
            case 0:
                fileInfo = new CasFileInfo();
                break;
            case 1:
                fileInfo = new CryptoCasFileInfo();
                break;
            case 2:
                fileInfo = new PatchFileInfo();
                break;
            case 3:
                fileInfo = new PathFileInfo();
                break;
            default:
                throw new InvalidDataException();
        }
        
        fileInfo.DeserializeInternal(stream);
        
        return fileInfo;
    }
}