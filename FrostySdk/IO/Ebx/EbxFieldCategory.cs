namespace Frosty.Sdk.IO.Ebx;

public enum EbxFieldCategory : byte
{
    None = 0,
    Pointer = 1,
    Struct = 2,
    PrimitiveType = 3,
    ArrayType = 4,
    EnumType = 5,
    FunctionType = 6,
    InterfaceType = 7,
    DelegateType = 8
}