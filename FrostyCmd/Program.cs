using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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

        // generate sdk if needed
        string sdkPath = $"Sdk/{ProfilesLibrary.SdkFilename}.dll";
        if (!File.Exists(sdkPath))
        {
            TypeSdkGenerator typeSdkGenerator = new();
            
            Process.Start(FileSystemManager.BasePath + ProfilesLibrary.ProfileName);
            
            // sleep 10 seconds to give ea time to launch the game
            Thread.Sleep(10 * 100);
            
            Process? game = null;
            while (game == null)
            {
                game = Process.GetProcessesByName(ProfilesLibrary.ProfileName).FirstOrDefault();
            }
            
            if (!typeSdkGenerator.DumpTypes(game))
            {
                throw new Exception("DumpTypes");
            }
            
            if (!typeSdkGenerator.CreateSdk(sdkPath))
            {
                throw new Exception("CreateSdk");
            }
        }

        // init type library, this loads the EbxTypeSdk used to properly parse ebx assets
        if (!TypeLibrary.Initialize())
        {
            throw new Exception("ProfilesLibrary");
        }
            
        // init filesystem manager, this parses the layout.toc file
        if (!FileSystemManager.Initialize("C:\\Program Files\\EA Games\\STAR WARS Battlefront II"))
        {
            throw new Exception("FileSystemManager");
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
        
        
    }
}