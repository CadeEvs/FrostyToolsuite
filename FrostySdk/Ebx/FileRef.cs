namespace Frosty.Sdk.Ebx;

public readonly struct FileRef
{
    private readonly string m_fileName = string.Empty;

    public FileRef(string value)
    {
        m_fileName = value;
    }

    public static implicit operator string(FileRef value) => value.m_fileName;

    public static implicit operator FileRef(string value) => new(value);

    public override string ToString() => $"FileRef '{m_fileName}'";

    public override bool Equals(object? obj)
    {
        if (obj is not FileRef b)
        {
            return false;
        }

        return Equals(b);
    }

    public bool Equals(FileRef b)
    {
        return m_fileName == b.m_fileName;
    }

    public static bool operator ==(FileRef a, object b) => a.Equals(b);

    public static bool operator !=(FileRef a, object b) => !a.Equals(b);

    public override int GetHashCode()
    {
        return m_fileName.GetHashCode();
    }
}