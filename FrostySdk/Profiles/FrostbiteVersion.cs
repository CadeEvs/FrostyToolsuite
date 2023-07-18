using System;

namespace Frosty.Sdk.Profiles;

public struct FrostbiteVersion
{
    private int m_major;
    private int m_minor;
    private int m_patch;

    public override string ToString()
    {
        return $"{m_major}.{m_minor}.{m_patch}";
    }

    public static implicit operator FrostbiteVersion(string value) => Parse(value);
    
    public static implicit operator string(FrostbiteVersion value) => value.ToString();

    public static bool operator <(FrostbiteVersion a, FrostbiteVersion b)
    {
        return a.m_major < b.m_major || (a.m_major == b.m_major &&
                                         (a.m_minor < b.m_minor || (a.m_minor == b.m_minor && a.m_patch < b.m_patch)));
    }
    
    public static bool operator >(FrostbiteVersion a, FrostbiteVersion b)
    {
        return a.m_major > b.m_major || (a.m_major == b.m_major &&
                                         (a.m_minor > b.m_minor || (a.m_minor == b.m_minor && a.m_patch > b.m_patch)));
    }
    
    public static bool operator <=(FrostbiteVersion a, FrostbiteVersion b)
    {
        return a < b || a == b;
    }
    
    public static bool operator >=(FrostbiteVersion a, FrostbiteVersion b)
    {
        return a > b || a == b;
    }
    
    public static bool operator ==(FrostbiteVersion a, FrostbiteVersion b)
    {
        return a.m_major == b.m_major && a.m_minor == b.m_minor && a.m_patch == b.m_patch;
    }
    
    public static bool operator !=(FrostbiteVersion a, FrostbiteVersion b)
    {
        return a.m_major != b.m_major && a.m_minor != b.m_minor && a.m_patch != b.m_patch;
    }
    
    public bool Equals(FrostbiteVersion other)
    {
        return m_major == other.m_major && m_minor == other.m_minor && m_patch == other.m_patch;
    }

    public override bool Equals(object? obj)
    {
        return obj is FrostbiteVersion other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(m_major, m_minor, m_patch);
    }

    private static FrostbiteVersion Parse(string value)
    {
        string[] splitted = value.Split('.');
        if (splitted.Length < 1)
        {
            throw new ArgumentException();
        }
        FrostbiteVersion version = new()
        {
            m_major = int.Parse(splitted[0])
        };
        if (splitted.Length > 1)
        {
            version.m_minor = int.Parse(splitted[1]);

            if (splitted.Length > 2)
            {
                version.m_patch = int.Parse(splitted[2]);   
            }   
        }
        return version;
    }
}