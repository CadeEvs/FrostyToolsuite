using FrostySdk.IO;

namespace FrostySdk.Interfaces
{
    public interface IDeobfuscator
    {
        long Initialize(NativeReader reader);
        bool AdjustPosition(NativeReader reader, long newPosition);
        void Deobfuscate(byte[] buffer, long position, int offset, int numBytes);
    }
}
