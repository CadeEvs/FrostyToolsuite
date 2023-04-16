using System;

namespace Frosty.Sdk.Ebx;

public readonly struct CString
{
    private readonly string m_strValue = string.Empty;

    public CString(string value = "") => m_strValue = value;

    public CString Sanitize() => new(m_strValue.Trim('\v', '\r', '\n', '\t'));

    public static implicit operator string(CString value) => value.m_strValue;

    public static implicit operator CString(string value) => new(value);

    public bool IsNullOrEmpty() => string.IsNullOrEmpty(m_strValue);

    public override string ToString() => m_strValue;

    public override int GetHashCode()
    {
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

    public bool Equals(CString cStr, StringComparison comparison)
    {
        return m_strValue.Equals(cStr.m_strValue, comparison);
    }
    
    public bool Equals(string str, StringComparison comparison)
    {
        return m_strValue.Equals(str, comparison);
    }

    public static bool operator ==(CString a, object b) => a.Equals(b);

    public static bool operator !=(CString a, object b) => !a.Equals(b);
}