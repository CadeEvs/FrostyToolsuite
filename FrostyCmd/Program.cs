using System;
using System.IO;
using Frosty.Sdk;
using Frosty.Sdk.IO.Ebx;
using Frosty.Sdk.Managers;
using Frosty.Sdk.Managers.Entries;

namespace FrostyCmd;

internal static class Program
{
    private static void Main()
    {
        // init profile
        if (!ProfilesLibrary.Initialize("GW2.Main_Win64_Retail"))
        {
            throw new Exception("ProfilesLibrary");
        }
            
        // init filesystem manager, this parses the layout.toc file
        if (!FileSystemManager.Initialize("E:\\FrostbiteGames\\Plants vs Zombies Garden Warfare 2"))
        {
            throw new Exception("FilesystemManager");
        }

        // init resource manager, this parses the cas.cat files if they exist for easy asset lookup
        if (!ResourceManager.Initialize())
        {
            throw new Exception("ResourceManager");
        }

        // init asset manager, this parses the SuperBundles and loads all the assets
        if (!AssetManager.Initialize())
        {
            throw new Exception("AssetManager");
        }

        EbxAssetEntry? ebxEntry = AssetManager.GetEbxAssetEntry("default/settings_win32");
        EbxAsset asset = AssetManager.GetEbx(ebxEntry!);
    }
}