using System.IO;
using Frosty.Sdk.IO;

namespace Frosty.Sdk.Interfaces;

public interface IDeobfuscator
{
    Stream? Initialize(DataStream stream);
}