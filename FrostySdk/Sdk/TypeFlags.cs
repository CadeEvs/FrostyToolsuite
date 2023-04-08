using System;

namespace Frosty.Sdk.Sdk;

public struct TypeFlags
{
    public enum TypeEnum
    {
        Inherited = 0x00,
        DbObject = 0x01,
        Struct = 0x02,
        Class = 0x03,
        Array = 0x04,
        String = 0x06,
        CString = 0x07,
        Enum = 0x08,
        FileRef = 0x09,
        Boolean = 0x0A,
        Int8 = 0x0B,
        UInt8 = 0x0C,
        Int16 = 0x0D,
        UInt16 = 0x0E,
        Int32 = 0x0F,
        UInt32 = 0x10,
        Int64 = 0x12,
        UInt64 = 0x11,
        Float32 = 0x13,
        Float64 = 0x14,
        Guid = 0x15,
        Sha1 = 0x16,
        ResourceRef = 0x17,
        Function = 0x18,
        TypeRef = 0x19,
        BoxedValueRef = 0x1A,
        Interface = 0x1B,
        Delegate = 0x1C
    }
    
    public enum CategoryEnum
    {
        None = 0,
        Class = 1,
        Struct = 2,
        Primitive = 3,
        Array = 4,
        Enum = 5,
        Function = 6,
        Interface = 7,
        Delegate = 8
    }

    [Flags]
    public enum Flags
    {
        MetaData = 1 << 11,
        Homogeneous = 1 << 12,
        AlwaysPersist = 1 << 13, // only valid on fields
        Exposed = 1 << 13, // only valid on fields
        FlagsEnum = 1 << 13, // enum is a bitfield only valid on enums
        LayoutImmutable = 1 << 14,
        Blittable = 1 << 15
    }

    private readonly ushort m_flags;

    public TypeFlags(ushort inFlags)
    {
        m_flags = inFlags;
    }

    public TypeFlags(TypeEnum type, CategoryEnum category = CategoryEnum.None)
    {
        m_flags = (ushort)((ushort)type << 4 | (ushort)category);
        if (ProfilesLibrary.EbxVersion != 2)
        {
            m_flags <<= 1;
        }
    }

    public TypeEnum GetTypeEnum() => (TypeEnum)((m_flags >> (ProfilesLibrary.EbxVersion == 2 ? 4 : 5)) & 0x1F);
    
    public CategoryEnum GetCategoryEnum() => (CategoryEnum)((m_flags >> (ProfilesLibrary.EbxVersion == 2 ? 0 : 1)) & 0xF);

    public Flags GetFlags() => (Flags)(m_flags & (ProfilesLibrary.EbxVersion == 2 ? 0x1FF : 0x3FF));

    public static implicit operator ushort(TypeFlags value) => value.m_flags;
    
    public static implicit operator TypeFlags(ushort value) => new(value);

}