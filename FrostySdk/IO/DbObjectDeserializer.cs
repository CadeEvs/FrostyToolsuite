using System;
using System.Collections.Generic;
using Frosty.Sdk.IO;

// ReSharper disable once CheckNamespace
namespace Frosty.Sdk;

public partial class DbObject
{
    public static DbObject? Deserialize(DataStream stream)
    {
        return (DbObject?)ReadDbObject(stream, out string _);
    }

    private static object? ReadDbObject(DataStream stream, out string objName)
    {
        objName = string.Empty;
        byte tmp = stream.ReadByte();

        DbType objType = (DbType)(tmp & 0x1F);
        if (objType == DbType.Invalid)
        {
            return null;
        }

        if ((tmp & 0x80) == 0)
        {
            objName = stream.ReadNullTerminatedString();
        }

        switch (objType)
        {
            case DbType.List:
                {
                    long size = stream.Read7BitEncodedLong();
                    long offset = stream.Position;

                    List<object> values = new();
                    while (stream.Position - offset < size)
                    {
                        object? subValue = ReadDbObject(stream, out string _);

                        if (subValue == null)
                        {
                            break;
                        }

                        values.Add(subValue);
                    }
                    return new DbObject(values);
                }

            case DbType.Object:
                {
                    long size = stream.Read7BitEncodedLong();
                    long offset = stream.Position;

                    Dictionary<string, object> values = new(StringComparer.OrdinalIgnoreCase);
                    while (stream.Position - offset < size)
                    {
                        object? subValue = ReadDbObject(stream, out string tmpName);

                        if (subValue == null)
                        {
                            break;
                        }

                        // TODO: this causes Small Object Heap issues
                        // https://www.jetbrains.com/help/rider/2022.3/Fixing_Issues_Found_by_DPA.html#small-object-heap
                        // no idea if u can even fix it since we need different types in the dict
                        // maybe we can just store some wrapper struct which underlying type is a byte[]
                        // and which has the As<T>() function from DbObjectExtensions.cs
                        // but this would mean that the usage like in the MeshSetPlugin would no longer work
                        // since we could only allow the types defined in DbType
                        values.Add(tmpName, subValue);
                    }
                    return new DbObject(values);
                }

            case DbType.Boolean:
                return stream.ReadByte() == 1;
            case DbType.String:
                return stream.ReadFixedSizedString(stream.Read7BitEncodedInt());
            case DbType.Int:
                return stream.ReadInt32();
            case DbType.Long:
                return stream.ReadInt64();
            case DbType.Float:
                return stream.ReadSingle();
            case DbType.Double:
                return stream.ReadDouble();
            case DbType.Guid:
                return stream.ReadGuid();
            case DbType.Sha1:
                return stream.ReadSha1();
            case DbType.ByteArray:
                return stream.ReadBytes(stream.Read7BitEncodedInt());
        }

        return null;
    }
}