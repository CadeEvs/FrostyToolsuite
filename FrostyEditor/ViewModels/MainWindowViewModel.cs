using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Controls;
using Dock.Model.Core;
using Frosty.Sdk;
using Frosty.Sdk.IO;
using Frosty.Sdk.Managers;
using FrostyEditor.ViewModels.Documents;

namespace FrostyEditor.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IDropTarget
{
    private readonly IFactory m_factory = new DockFactory();

    private readonly FileViewModel m_defaultPage = new DefaultPageViewModel { Title = "Home" };
    
    [ObservableProperty]
    private IRootDock? m_layout;

    public MainWindowViewModel()
    {
        // Setup Docking
        Layout = m_factory.CreateLayout();
        if (Layout is not null)
        {
            m_factory.InitLayout(Layout);
        }
    }

    public static async Task<bool> SetupFrostySdk(string inProfileKey, string inProfilePath)
    {
        return await Task.Run(() =>
        {
            if (!ProfilesLibrary.Initialize(inProfileKey))
            {
                return false;
            }

            if (ProfilesLibrary.RequiresKey)
            {
                // TODO: key window
                string keyPath = $"Keys/{ProfilesLibrary.InternalName}.key";
                if (File.Exists(keyPath))
                {
                    using (BlockStream stream = BlockStream.FromFile(keyPath, false))
                    {
                        if (stream.Length < 0x10)
                        {
                            return false;
                        }
                        
                        byte[] initFsKey = new byte[0x10];
                        stream.ReadExactly(initFsKey);
                        KeyManager.AddKey("InitFsKey", initFsKey);

                        if (stream.Length > 0x10)
                        {
                            // TODO: add other keys
                        }
                    }
                }
            }
                
            // init filesystem manager, this parses the layout.toc file
            if (!FileSystemManager.Initialize(inProfilePath))
            {
                return false;
            }

            // generate sdk if needed
            string sdkPath = $"Sdk/{ProfilesLibrary.SdkFilename}.dll";
            // if (!File.Exists(sdkPath))
            // {
            //     TypeSdkGenerator typeSdkGenerator = new();
            //     
            //     Process.Start($"{FileSystemManager.BasePath}{ProfilesLibrary.ProfileName}");
            //     
            //     // sleep 10 seconds to give ea time to launch the game
            //     Thread.Sleep(10 * 100);
            //     
            //     Process? game = null;
            //     while (game is null)
            //     {
            //         game = Process.GetProcessesByName(ProfilesLibrary.ProfileName).FirstOrDefault();
            //     }
            //     
            //     if (!typeSdkGenerator.DumpTypes(game))
            //     {
            //         return false;
            //     }
            //     
            //     if (!typeSdkGenerator.CreateSdk(sdkPath))
            //     {
            //         return false;
            //     }
            // }
            //
            // // init type library, this loads the EbxTypeSdk used to properly parse ebx assets
            // if (!TypeLibrary.Initialize())
            // {
            //     return false;
            // }
            
            // init resource manager, this parses the cas.cat files if they exist for easy asset lookup
            if (!ResourceManager.Initialize())
            {
                return false;
            }
            
            // init asset manager, this parses the SuperBundles and loads all the assets
            if (!AssetManager.Initialize())
            {
                return false;
            }

            return true;
        });
    }

    private void AddFileViewModel(FileViewModel fileViewModel)
    {
        IDocumentDock? files = m_factory.GetDockable<IDocumentDock>("Files");
        if (Layout is not null && files is not null)
        {
            m_factory.AddDockable(files, fileViewModel);
            m_factory.SetActiveDockable(fileViewModel);
            m_factory.SetFocusedDockable(Layout, fileViewModel);
        }
    }

    private FileViewModel? GetActiveFileViewModel()
    {
        IDocumentDock? files = m_factory.GetDockable<IDocumentDock>("Files");
        return files?.ActiveDockable as FileViewModel;
    }

    private FileViewModel GetDefaultPageViewModel() => m_defaultPage;

    public void CloseLayout()
    {
        if (Layout is IDock dock)
        {
            if (dock.Close.CanExecute(null))
            {
                dock.Close.Execute(null);
            }
        }
    }

    public void DragOver(object? sender, DragEventArgs e)
    {
        if (!e.Data.Contains(DataFormats.Files))
        {
            e.DragEffects = DragDropEffects.None; 
            e.Handled = true;
        }
    }

    public void Drop(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Files))
        {
            IEnumerable<IStorageItem>? result = e.Data.GetFiles();
            if (result is not null)
            {
                foreach (IStorageItem path in result)
                {
                    if (!string.IsNullOrEmpty(path.Path.LocalPath))
                    {
                        // check if its a fbproject and then load it
                    }
                }
            }
            e.Handled = true;
        }
    }
}
