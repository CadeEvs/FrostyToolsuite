using System.Collections.Generic;

namespace Frosty.Sdk.Managers;

public static class KeyManager
{
    private static readonly Dictionary<int, byte[]> s_keys = new();

    public static void AddKey(string id, byte[] data)
    {
        int hash = Utils.Utils.HashString(id);
        s_keys.TryAdd(hash, data);
    }

    public static byte[] GetKey(string id)
    {
        int hash = Utils.Utils.HashString(id);
        if (!s_keys.ContainsKey(hash))
        {
            throw new KeyNotFoundException($"Could not find key with id: {id}");
        }

        return s_keys[hash];
    }

    public static bool HasKey(string id) => s_keys.ContainsKey(Utils.Utils.HashString(id));
}