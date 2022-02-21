using System.Collections.Generic;

namespace FrostySdk.Managers
{
    public class KeyManager
    {
        private Dictionary<string, byte[]> keys = new Dictionary<string, byte[]>();

        public static KeyManager Instance { get; } = new KeyManager();

        private KeyManager()
        {
        }

        public void AddKey(string id, byte[] data)
        {
            if (!keys.ContainsKey(id))
                keys.Add(id, null);
            keys[id] = data;
        }

        public byte[] GetKey(string id) => !keys.ContainsKey(id) ? null : keys[id];

        public bool HasKey(string id) => keys.ContainsKey(id);
    }
}
