using Frosty.Core.IO;
using FrostySdk;
using FrostySdk.IO;

namespace Frosty.Core.Mod
{
    // internally used for handling legacy asset changes.
    internal interface ILegacyCustomActionHandler : IModCustomActionHandler
    {
        void SaveToMod(FrostyModWriter writer);
        bool SaveToProject(NativeWriter writer);
        void LoadFromProject(DbObject project);
        void LoadFromProject(uint version, NativeReader reader, string type);
    }
}
