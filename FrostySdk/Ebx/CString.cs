using System;

namespace Frosty.Sdk.Ebx;

public struct CString
{
    private string m_strValue = string.Empty;

    public CString(string value = "") => m_strValue = value;

    public CString Sanitize() => new(m_strValue.Trim('\v', '\r', '\n', '\t'));

    public static implicit operator string(CString value) => value.m_strValue;

    public static implicit operator CString(string value) => new(value);

    public bool IsNull() => m_strValue == null;

    public override string ToString() => m_strValue;

    public override int GetHashCode()
    {
        if (m_strValue == null)
            return "".GetHashCode();

        return m_strValue.GetHashCode();
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is CString cStr)
        {
            return m_strValue.Equals(cStr.m_strValue);
        }
        
        if (obj is string str)
        {
            return m_strValue.Equals(str);
        }
        return false;
    }

    public bool Equals(object? obj, StringComparison comparison)
    {
        if (obj is CString cStr)
        {
            return m_strValue.Equals(cStr.m_strValue, comparison);
        }
        
        if (obj is string str)
        {
            return m_strValue.Equals(str, comparison);
        }
        return false;
    }
}