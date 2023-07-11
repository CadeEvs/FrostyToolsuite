using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Frosty.Sdk;
using Frosty.Sdk.Managers;
using Frosty.Sdk.Sdk;

namespace FrostyCmd;

internal static class Program
{
    private static byte[] s_key = {
        
    };

    /// <summary>
    /// The name of the games executable
    /// </summary>
    private const string c_profileKey = "Anthem";
    
    /// <summary>
    /// Path of the games base directory
    /// </summary>
    private const string c_profilePath = ".../Anthem";
    
    /// <summary>
    /// Exist bc i dont want to launch the games and the sdk is only needed for ebx reading
    /// </summary>
    private const bool c_generateSdk = false;
    
    private static void Main()
    {
        // init profile
        if (!ProfilesLibrary.Initialize(c_profileKey))
        {
            throw new Exception("ProfilesLibrary");
        }
        
        // add key for initFs, only needed for newer games (ProfilesLibrary.RequiresKey)
        KeyManager.AddKey("InitFsKey", s_key);
            
        // init filesystem manager, this parses the layout.toc file
        if (!FileSystemManager.Initialize(c_profilePath))
        {
            throw new Exception("FileSystemManager");
        }

        if (c_generateSdk)
        {
            // generate sdk if needed
            string sdkPath = $"Sdk/{ProfilesLibrary.SdkFilename}.dll";
            if (!File.Exists(sdkPath))
            {
                TypeSdkGenerator typeSdkGenerator = new();
            
                Process.Start(FileSystemManager.BasePath + ProfilesLibrary.ProfileName);
            
                // sleep 10 seconds to give ea time to launch the game
                Thread.Sleep(10 * 100);
            
                Process? game = null;
                while (game is null)
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