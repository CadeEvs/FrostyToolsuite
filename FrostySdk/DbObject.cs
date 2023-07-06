using System;
using System.IO;
using Frosty.Sdk.DbObjectElements;
using Frosty.Sdk.IO;

namespace Frosty.Sdk;

public abstract class DbObject
{
    [Flags]
    protected internal enum Type
    {
        Null = 0,
        List = 1,
        Dict = 2,
        Boolean = 6,
        String = 7,
        Int = 8,
        Long = 9,
        Float = 11,
        Double = 12,
        Guid = 15,
        Sha1 = 16,
        Blob = 19,
        
        Anonymous = 1 << 7
    }
    
    public string Name { get; private set; }

    private readonly Type m_type;

    protected DbObject(Type inType)
    {
        Name = string.Empty;
        m_type = inType;
    }

    protected DbObject(Type inType, string inName)
        : this(inType)
    {
        Name = inName;
    }

    /// <summary>
    /// Serializes a <see cref="DbObject"/> to a file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="value">The <see cref="DbObject"/> to serialize.</param>
    public static void Serialize(string path, DbObject value)
    {
        using (DataStream stream = new(new FileStream(path, FileMode.Create, FileAccess.ReadWrite)))
        {
            Serialize(stream, value);
        }
    }
    
    /// <summary>
    /// Serializes a <see cref="DbObject"/> to a <see cref="DataStream"/>.
    /// </summary>
    /// <param name="stream">The <see cref="DataStream"/> to serialize the <see cref="DbObject"/> to.</param>
    /// <param name="value">The <see cref="DbObject"/> to serialize.</param>
    public static void Serialize(DataStream stream, DbObject value)
    {
        stream.WriteByte((byte)value.m_type);
        
        if (!value.m_type.HasFlag(Type.Anonymous))
        {
            stream.WriteNullTerminatedString(value.Name);
        }
        
        value.InternalSerialize(stream);
    }

    /// <summary>
    /// Deserializes a <see cref="DbObject"/> from a file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>The deserialized <see cref="DbObject"/>.</returns>
    public static DbObject? Deserialize(string path)
    {
        using (BlockStream stream = BlockStream.FromFile(path, true))
        {
            return Deserialize(stream);
        }
    }

    /// <summary>
    /// Deserializes a <see cref="DbObject"/> from a <see cref="DataStream"/>.
    /// </summary>
    /// <param name="stream">The <see cref="DataStream"/> to deserialize the <see cref="DbObject"/> from.</param>
    /// <returns>The deserialized <see cref="DbObject"/>.</returns>
    public static DbObject? Deserialize(DataStream stream)
    {
        Type type = (Type)stream.ReadByte();
        
        DbObject? obj = CreateDbObject(type);

        if (obj is null)
        {
            return obj;
        }
        
        if (!type.HasFlag(Type.Anonymous))
        {
            obj.Name = stream.ReadNullTerminatedString();
        }
        
        obj.InternalDeserialize(stream);

        return obj;
    }

    public virtual bool IsDict() => false;
    
    public virtual DbObjectDict AsDict()
    {
        throw new Exception();
    }

    public virtual bool IsList() => false;
    
    public virtual DbObjectList AsList()
    {
        throw new Exception();
    }

    public virtual bool AsBoolean()
    {
        throw new Exception();
    }

    public virtual string AsString()
    {
        throw new Exception();
    }

    public virtual int AsInt()
    {
        throw new Exception();
    }

    public virtual uint AsUInt()
    {
        throw new Exception();
    }

    public virtual long AsLong()
    {
        throw new Exception();
    }

    public virtual ulong AsULong()
    {
        throw new Exception();
    }

    public virtual float AsFloat()
    {
        throw new Exception();
    }

    public virtual double AsDouble()
    {
        throw new Exception();
    }

    public virtual Guid AsGuid()
    {
        throw new Exception();
    }

    public virtual Sha1 AsSha1()
    {
        throw new Exception();
    }

    public virtual byte[] AsBlob()
    {
        throw new Exception();
    }

    public static DbObjectDict CreateDict(int capacity = 0) => new(capacity);
    public static DbObjectDict CreateDict(string name, int capacity = 0) => new(name, capacity);
    public static DbObjectList CreateList(int capacity = 0) => new(capacity);
    public static DbObjectList CreateList(string name, int capacity = 0) => new(name, capacity);
    
    protected abstract void InternalSerialize(DataStream stream);
    
    protected abstract void InternalDeserialize(DataStream stream);

    private static DbObject? CreateDbObject(Type type)
    {
        DbObject obj;
        switch (type & ~Type.Anonymous)
        {
            case Type.Null:
                return null;
            case Type.List:
                obj = new DbObjectList(type);
                break;
            case Type.Dict:
                obj = new DbObjectDict(type);
                break;
            case Type.Boolean:
                obj = new DbObjectBool(type);
                break;
            case Type.String:
                obj = new DbObjectString(type);
                break;
            case Type.Int:
                obj = new DbObjectInt(type);
                break;
            case Type.Long:
                obj = new DbObjectLong(type);
                break;
            case Type.Float:
                obj = new DbObjectFloat(type);
                break;
            case Type.Double:
                obj = new DbObjectDouble(type);
                break;
            case Type.Guid:
                obj = new DbObjectGuid(type);
                break;
            case Type.Sha1:
                obj = new DbObjectSha1(type);
                break;
            case Type.Blob:
                obj = new DbObjectBlob(type);
                break;
            default:
                throw new Exception();
        }

        return obj;
    }
}