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
}