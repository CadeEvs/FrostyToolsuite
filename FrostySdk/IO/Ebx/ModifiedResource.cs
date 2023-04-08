using System;
using System.IO;

namespace Frosty.Sdk.IO.Ebx;


public class ModifiedResource
{
    public ModifiedResource()
    {
    }

    public byte[] Save()
    {
        using (DataStream stream= new(new MemoryStream()))
        {
            stream.WriteNullTerminatedString(GetType().AssemblyQualifiedName!);
            using (DataStream subStream = new(new MemoryStream()))
            {
                SaveInternal(subStream);
                stream.Write(subStream.ToByteArray(), 0, (int)subStream.Length);
            }

            return stream.ToByteArray();;
        }
    }

    protected virtual void SaveInternal(DataStream stream)
    {
    }

    public static ModifiedResource Read(byte[] buffer)
    {
        using (DataStream stream = new(new MemoryStream(buffer)))
        {
            string type = FixupTypes(stream.ReadNullTerminatedString());
            using (DataStream subStream = new(new MemoryStream(stream.ToByteArray())))
            {
                ModifiedResource md = (ModifiedResource)Activator.CreateInstance(Type.GetType(type)!)!;
                md.ReadInternal(subStream);

                return md;
            }
        }
    }

    protected virtual void ReadInternal(DataStream stream)
    {
    }

    private static string FixupTypes(string type)
    {
        return type == "ModifiedShaderBlockDepot" ? "MeshSetPlugin.Resources.ModifiedShaderBlockDepot, MeshSetPlugin" : type;
    }
}