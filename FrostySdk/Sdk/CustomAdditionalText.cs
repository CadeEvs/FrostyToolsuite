using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Frosty.Sdk.Sdk;

public class CustomAdditionalText : AdditionalText
{
    private readonly string m_text;

    public override string Path { get; }

    public CustomAdditionalText(string path)
    {
        Path = path;
        m_text = File.ReadAllText(path);
    }

    public override SourceText GetText(CancellationToken cancellationToken = new())
    {
        return SourceText.From(m_text);
    }
}