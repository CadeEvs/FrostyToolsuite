using System;

namespace Frosty.Sdk.Utils.CompressionTypes;

public static class ZStd
{
    private static byte[]? s_dictionary;

    public static byte[] GetDictionary()
    {
        if (s_dictionary == null)
        {
            throw new Exception("Dictionary doesn't exit.");
        }

        return s_dictionary;
    }

    public static void SetDictionary(byte[] dict)
    {
        s_dictionary = dict;
    }
}