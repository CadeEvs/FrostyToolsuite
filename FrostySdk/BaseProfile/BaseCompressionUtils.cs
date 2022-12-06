using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrostySdk.BaseProfile
{
    public class BaseCompressionUtils : ICompressionUtils
    {
        public string GetOodleDllName(string basePath)
        {
            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19, ProfileVersion.Fifa20, ProfileVersion.PlantsVsZombiesBattleforNeighborville, ProfileVersion.NeedForSpeedHeat))
            {
                return basePath + "oo2core_6_win64.dll";
            }
            else if (ProfilesLibrary.IsLoaded(ProfileVersion.Anthem))
            {
                return basePath + "oo2core_7_win64.dll";
            }
            else if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21, ProfileVersion.Madden22, ProfileVersion.Fifa22, ProfileVersion.Battlefield2042, ProfileVersion.Madden23))
            {
                return basePath + "oo2core_8_win64.dll";
            }
            else if (ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedUnbound))
            {
                return basePath + "oo2core_9_win64.dll";
            }
            else
            {
                return basePath + "oo2core_4_win64.dll";
            }
        }

        public string GetZStdDllName()
        {
            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17))
            {
                return "thirdparty/libzstd.0.0.6.dll";
            }
            else if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19, ProfileVersion.Fifa20, ProfileVersion.Fifa21, ProfileVersion.Fifa22))
            {
                return "thirdparty/libzstd.1.3.4.dll";
            }
            else
            {
                return "thirdparty/libzstd.1.1.5.dll";
            }
        }

        public bool LoadOodle => ProfilesLibrary.IsLoaded(ProfileVersion.Fifa18, ProfileVersion.Fifa19, ProfileVersion.Anthem,
            ProfileVersion.Fifa20, ProfileVersion.PlantsVsZombiesBattleforNeighborville, ProfileVersion.NeedForSpeedHeat,
            ProfileVersion.Fifa21, ProfileVersion.Madden22, ProfileVersion.Fifa22,
            ProfileVersion.Battlefield2042, ProfileVersion.Madden23, ProfileVersion.NeedForSpeedUnbound);

        public bool LoadZStd => ProfilesLibrary.IsLoaded(ProfileVersion.MassEffectAndromeda, ProfileVersion.Fifa17, ProfileVersion.Fifa18,
            ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Madden19, ProfileVersion.Fifa19,
            ProfileVersion.Battlefield5, ProfileVersion.Fifa20, ProfileVersion.StarWarsSquadrons,
            ProfileVersion.Fifa21, ProfileVersion.Madden22, ProfileVersion.Fifa22,
            ProfileVersion.Battlefield2042, ProfileVersion.Madden23, ProfileVersion.NeedForSpeedUnbound);

        public int OodleCompressionLevel => ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons) ? 18 : 16;
    }
}
