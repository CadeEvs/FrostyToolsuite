using System;
using System.Text;
using Frosty.Sdk.IO.Ebx;
using Frosty.Sdk.Sdk;

namespace Frosty.Sdk.Ebx;

public class BoxedValueRef
{
    public object? Value => m_value;
    public TypeFlags.TypeEnum Type => m_type;
    public TypeFlags.TypeEnum ArrayType => m_subType;
    public EbxFieldCategory Category => m_category;
    public string TypeString
    {
        get
        {
            switch (m_type)
            {
                case TypeFlags.TypeEnum.Array:
                    return EbxTypeToString(m_subType, m_value!.GetType().GenericTypeArguments[0]);
                case TypeFlags.TypeEnum.Enum:
                case TypeFlags.TypeEnum.Struct:
                    return m_value!.GetType().Name;
                case TypeFlags.TypeEnum.CString:
                    return "CString";
                default:
                    return m_type.ToString();
            }
        }
    }

    private object? m_value;
    private readonly TypeFlags.TypeEnum m_type;
    private readonly TypeFlags.TypeEnum m_subType;
    private readonly EbxFieldCategory m_category;

    public BoxedValueRef()
    {
    }

    public BoxedValueRef(object? inval, TypeFlags.TypeEnum intype)
    {
        m_value = inval;
        m_type = intype;
    }

    public BoxedValueRef(object? inval, TypeFlags.TypeEnum intype, TypeFlags.TypeEnum insubtype)
    {
        m_value = inval;
        m_type = intype;
        m_subType = insubtype;
    }

    public BoxedValueRef(object? inval, TypeFlags.TypeEnum intype, TypeFlags.TypeEnum insubtype, EbxFieldCategory incategory)
    {
        m_value = inval;
        m_type = intype;
        m_subType = insubtype;
        m_category = incategory;
    }

    public void SetValue(object invalue)
    {
        m_value = invalue;
    }

    public override string ToString()
    {
        if (Value == null)
            return "BoxedValueRef '(null)'";
        string s = "BoxedValueRef '";
        switch (m_type)
        {
            case TypeFlags.TypeEnum.Array:
                s += $"Array<{EbxTypeToString(m_subType, m_value.GetType().GenericTypeArguments[0])}>";
                break;
            case TypeFlags.TypeEnum.Enum:
                s += $"{m_value.GetType().Name}";
                break;
            case TypeFlags.TypeEnum.Struct:
                s += $"{m_value.GetType().Name}";
                break;
            case TypeFlags.TypeEnum.CString:
                s += "CString";
                break;
            default:
                s += $"{m_type}";
                break;
        }

        return $"{s}'";
    }

    private string EbxTypeToString(TypeFlags.TypeEnum typeToConvert, Type actualType)
    {
        switch (typeToConvert)
        {
            case TypeFlags.TypeEnum.Enum:
            case TypeFlags.TypeEnum.Struct:
                return actualType.Name;
            case TypeFlags.TypeEnum.CString:
                return "CString";
            default:
                return typeToConvert.ToString();
        }
    }
}