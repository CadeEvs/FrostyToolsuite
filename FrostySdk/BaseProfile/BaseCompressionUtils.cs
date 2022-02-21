using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrostySdk.BaseProfile
{
    public class BaseCompressionUtils : ICompressionUtils
    {
        public string GetOodleDllName(string basePath)
        {
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat)
                return basePath + "oo2core_6_win64.dll";
            else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem)
                return basePath + "oo2core_7_win64.dll";
            else
                return basePath + "oo2core_4_win64.dll"; ;
        }

        public string GetZStdDllName()
        {
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17)
                return "thirdparty/libzstd.0.0.6.dll";
            else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20)
                return "thirdparty/libzstd.1.3.4.dll";
            else
                return "thirdparty/libzstd.1.1.5.dll";
        }

        public bool LoadOodle => ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat;

        public bool LoadZStd => ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons;

        public int OodleCompressionLevel => (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || (ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)) ? 18 : 16;
    }
}
