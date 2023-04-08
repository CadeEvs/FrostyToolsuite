using System.Collections.Generic;

namespace Frosty.Sdk.Sdk;

internal static class Strings
{
    public static readonly Dictionary<uint, string> StringHashes = new();
    public static readonly Dictionary<uint, string> ClassHashes = new();
    public static readonly Dictionary<uint, Dictionary<uint, string>> FieldHashes = new();
    
}