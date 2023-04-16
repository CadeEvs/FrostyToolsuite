using System;
using System.Diagnostics;
using System.Linq;
using Frosty.Sdk;
using Frosty.Sdk.IO.Ebx;
using Frosty.Sdk.Managers;
using Frosty.Sdk.Managers.Entries;
using Frosty.Sdk.Sdk;

namespace FrostyCmd;

internal static class Program
{
    private static void Main()
    {
        // init profile
        if (!ProfilesLibrary.Initialize("starwarsbattlefrontii"))
        {
            throw new Exception("ProfilesLibrary");
        }
            
        // init filesystem manager, this parses the layout.toc file
        if (!FileSystemManager.Initialize("C:\\Program Files\\EA Games\\STAR WARS Battlefront II"))
        {
            throw new Exception("FileSystemManager");
        }
        
        TypeSdkGenerator typeSdkGenerator = new();

        //Process.Start(FileSystemManager.BasePath + ProfilesLibrary.ProfileName);
        Process? game = null;
        while (game == null)
        {
            game = Process.GetProcessesByName(ProfilesLibrary.ProfileName).FirstOrDefault();
        }

        if (!typeSdkGenerator.DumpTypes(game))
        {
            throw new Exception("dumping types");
        }

        if (!typeSdkGenerator.CreateSdk())
        {
            throw new Exception("sdk writing");
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