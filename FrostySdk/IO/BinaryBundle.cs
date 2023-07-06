using System;
using System.IO;
using System.Text;
using Frosty.Sdk.Managers.Loaders;
using Frosty.Sdk.Profiles;

namespace Frosty.Sdk.IO;

public partial class BinaryBundle
{
    public enum Magic : uint
    {
        Standard = 0xED1CEDB8,
        Fifa = 0xC3889333,
        Encrypted = 0xC3E5D5C3
    }
    
    /// <summary>
    /// Dependent on the FB version, games have different salts.
    /// If the game uses a version newer than 2017 it uses "pecn", else it uses "pecm".
    /// <see cref="ProfileVersion.Battlefield5"/> is the only game that uses "arie".
    /// </summary>
    /// <returns>The salt, that the current game uses.</returns>
    public static uint GetSalt()
    {
        string salt = "pecm";

        if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5))
        {
            salt = "arie";
        }

        else if (ProfilesLibrary.DataVersion >= (int)ProfileVersion.Fifa19 && !ProfilesLibrary.IsLoaded(ProfileVersion.StarWarsSquadrons))
        {
            salt = "pecn";
        }


        byte[] bytes = Encoding.ASCII.GetBytes(salt);
        return (uint)(bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3] << 0);
    }

    /// <summary>
    /// Only the games using the <see cref="KelvinAssetLoader"/> use a different Magic than <see cref="Magic.Standard"/>.
    /// </summary>
    /// <returns>The magic the current game uses.</returns>
    public static Magic GetMagic()
    {
        switch (ProfilesLibrary.DataVersion)
        {
            // TODO: what game uses encrypted magic
            case (int)ProfileVersion.Fifa19:
            case (int)ProfileVersion.Madden20:
            case (int)ProfileVersion.Fifa20:
            case (int)ProfileVersion.Madden21:
                return Magic.Fifa;
            default:
                return Magic.Standard;
        }
    }

    public static bool IsValidMagic(Magic magic) => Enum.IsDefined(typeof(Magic), magic);
    
    /// <summary>
    /// Deserialize a binary stored bundle as <see cref="DbObject"/>.
    /// </summary>
    /// <param name="stream">The <see cref="DataStream"/> from which the bundle should be Deserialized from.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">Is thrown when there is no valid <see cref="Magic"/>.</exception>
    public static BinaryBundle Deserialize(DataStream stream)
    {
        return new BinaryBundle(stream);
    }
}