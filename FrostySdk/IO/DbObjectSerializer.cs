using System;
using System.Collections.Generic;
using System.IO;
using Frosty.Sdk.IO;

// ReSharper disable once CheckNamespace
namespace Frosty.Sdk;

public partial class DbObject
{
    /// <summary>
    /// Serializes a <see cref="DbObject"/> to a file.
    /// </summary>
    /// <param name="fileName">The name of the file to serialize to.</param>
    /// <param name="dbObject">The <see cref="DbObject"/> to serialize.</param>
    public static void Serialize(string fileName, DbObject dbObject)
    {
        using (DataStream stream = new DataStream(new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite)))
        {
            Serialize(stream, dbObject);
            // TODO: obfuscate stream
        }
    }
    
    /// <summary>
    /// Serializes a <see cref="DbObject"/> to a <see cref="DataStream"/>.
    /// </summary>
    /// <param name="stream">The <see cref="DataStream"/> to serialize to.</param>
    /// <param name="dbObject">The <see cref="DbObject"/> to serialize.</param>
    public static void Serialize(DataStream stream, DbObject dbObject)
    {
        WriteDbObject(stream, string.Empty, dbObject);
    }
    
    private static void WriteDbObject(DataStream stream, string name, object inObj)
    {
        DbType objType = GetDbType(inObj);

        byte dbFlags = (byte)(string.IsNullOrEmpty(name) ? 0x80 : 0x00);
        stream.WriteByte((byte)(dbFlags | (byte)objType));

        if ((dbFlags & 0x80) == 0)
        {
            stream.WriteNullTerminatedString(name);
        }

        switch (objType)
        {
            case DbType.Object:
                {
                    DbObject dbObj = (DbObject)inObj;
                    using DataStream subMs = new(new MemoryStream());

                    foreach (KeyValuePair<string, object> kvp in dbObj.m_hash!)
                    {
                        WriteDbObject(subMs, kvp.Key, kvp.Value);
                    }

                    byte[] buffer = subMs.ToByteArray();
                    stream.Write7BitEncodedLong(buffer.Length + 1);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.WriteByte(0x00);
                }
                break;

            case DbType.List:
                {
                    DbObject dbObj = (DbObject)inObj;
                    using DataStream subMs = new(new MemoryStream());

                    foreach (object obj in dbObj.m_list!)
                    {
                        WriteDbObject(subMs, string.Empty, obj);
                    }

                    byte[] buffer = subMs.ToByteArray();
                    stream.Write7BitEncodedLong(buffer.Length + 1);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.WriteByte(0x00);
                }
                break;

            case DbType.Boolean:
                stream.WriteBoolean((bool)inObj); break;
            case DbType.String:
                stream.WriteSizedString(((string)inObj) + "\0"); break;
            case DbType.Int:
                stream.WriteInt32((int)inObj); break;
            case DbType.Long:
                stream.WriteInt64((long)inObj); break;
            case DbType.Float:
                stream.WriteSingle((float)inObj); break;
            case DbType.Double:
                stream.WriteDouble((double)inObj); break;
            case DbType.Guid:
                stream.WriteGuid((Guid)inObj); break;
            case DbType.Sha1:
                stream.WriteSha1((Sha1)inObj); break;
            case DbType.ByteArray:
                stream.Write7BitEncodedInt(((byte[])inObj).Length);
                stream.Write((byte[])inObj, 0, ((byte[])inObj).Length);
                break;
            default:
                throw new InvalidDataException("Unsupported DB type detected");
        }
    }
    
    private static DbType GetDbType(object inObj)
    {
        Type objType = inObj.GetType();
        if (objType == typeof(DbObject))
        {
            DbObject obj = (DbObject)inObj;
            return obj.GetDbType();
        }
        else if (objType == typeof(bool))
            return DbType.Boolean;
        else if (objType == typeof(string))
            return DbType.String;
        else if (objType == typeof(int))
            return DbType.Int;
        else if (objType == typeof(long))
            return DbType.Long;
        else if (objType == typeof(float))
            return DbType.Float;
        else if (objType == typeof(double))
            return DbType.Double;
        else if (objType == typeof(Guid))
            return DbType.Guid;
        else if (objType == typeof(Sha1))
            return DbType.Sha1;
        else if (objType == typeof(byte[]))
            return DbType.ByteArray;

        return DbType.Invalid;
    }
}