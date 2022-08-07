using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Frosty.Core;
using FrostySdk.IO;
using FrostySdk.Managers;
using LevelEditorPlugin.Assets;

namespace LevelEditorPlugin.Managers
{
    public class LoadedAssetManager
    {
        #region -- Singleton --

        public static LoadedAssetManager Instance { get; private set; } = new LoadedAssetManager();
        private LoadedAssetManager() { }

        #endregion

        private struct ModificationState
        {
            public bool IsModified;
            public bool IsDirtied;
        }

        private class LoadedAssetInfo
        {
            public Asset LoadedAsset;
            public EbxAsset EbxAsset;
            public int RefCount;
            public Stack<ModificationState> ModificationState = new Stack<ModificationState>();

            public void Increment() { RefCount++; }
            public void Decrement() { RefCount--; }
        }

        private Dictionary<EbxImportReference, LoadedAssetInfo> loadedAssets = new Dictionary<EbxImportReference, LoadedAssetInfo>();
        private Dictionary<Guid, LoadedAssetInfo> loadedEbx = new Dictionary<Guid, LoadedAssetInfo>();

        public T LoadAsset<T>(Guid assetFileGuid) where T : Asset
        {
            EbxAssetEntry entry = App.AssetManager.GetEbxEntry(assetFileGuid);
            if (entry == null)
                return default(T);
            return LoadAsset<T>(new EbxImportReference() { FileGuid = entry.Guid, ClassGuid = Guid.Empty });
        }

        public T LoadAsset<T>(EbxImportReference importRefr) where T : Asset
        {
            if (!loadedAssets.ContainsKey(importRefr))
            {
                EbxAsset ebxAsset = (loadedEbx.ContainsKey(importRefr.FileGuid))
                    ? loadedEbx[importRefr.FileGuid].EbxAsset
                    : App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(importRefr.FileGuid));

                if (importRefr.ClassGuid == Guid.Empty)
                {
                    importRefr.ClassGuid = ebxAsset.RootInstanceGuid;
                }

                if (!loadedAssets.ContainsKey(importRefr))
                {
                    if (!loadedEbx.ContainsKey(importRefr.FileGuid))
                    {
                        EbxAssetEntry entry = App.AssetManager.GetEbxEntry(importRefr.FileGuid);
                        loadedEbx.Add(importRefr.FileGuid, new LoadedAssetInfo() { EbxAsset = ebxAsset });
                        PushState(entry, loadedEbx[importRefr.FileGuid], true);
                    }

                    object obj = ebxAsset.GetObject(importRefr.ClassGuid);
                    Asset loadedAsset = CreateAsset(obj as FrostySdk.Ebx.Asset, importRefr.FileGuid);

                    if (loadedAsset == null || !(loadedAsset is T))
                    {
                        UnloadEbx(importRefr.FileGuid);
                        return default(T);
                    }

                    loadedAssets.Add(importRefr, new LoadedAssetInfo() { LoadedAsset = loadedAsset });
                }
            }

            loadedAssets[importRefr].Increment();
            loadedEbx[importRefr.FileGuid].Increment();

            return loadedAssets[importRefr].LoadedAsset as T;
        }

        public T LoadAsset<T>(Entities.IEbxType owner, FrostySdk.Ebx.PointerRef pointer) where T : Asset
        {
            if (pointer.Type == PointerRefType.Null)
                return null;

            if (pointer.Type == PointerRefType.Internal)
                return LoadAsset<T>(owner.FileGuid, pointer.Internal as FrostySdk.Ebx.Asset);

            return LoadAsset<T>(pointer.External);
        }

        public T LoadAsset<T>(string assetPath) where T : Asset
        {
            EbxAssetEntry entry = App.AssetManager.GetEbxEntry(assetPath);
            if (entry == null)
                return default(T);
            return LoadAsset<T>(new EbxImportReference() { FileGuid = entry.Guid, ClassGuid = Guid.Empty });
        }

        public T LoadAsset<T>(Guid assetFileGuid, FrostySdk.Ebx.Asset assetData, EbxAsset ebxAsset = null) where T : Asset
        {
            EbxImportReference importRefr = new EbxImportReference()
            {
                FileGuid = assetFileGuid,
                ClassGuid = assetData.__InstanceGuid.ExportedGuid
            };

            if (!loadedAssets.ContainsKey(importRefr))
            {
                if (!loadedEbx.ContainsKey(importRefr.FileGuid))
                {
                    Debug.Assert(ebxAsset != null);
                    EbxAssetEntry entry = App.AssetManager.GetEbxEntry(ebxAsset.FileGuid);
                    loadedEbx.Add(importRefr.FileGuid, new LoadedAssetInfo() { EbxAsset = ebxAsset });
                    PushState(entry, loadedEbx[importRefr.FileGuid], true);
                }

                Asset loadedAsset = CreateAsset(assetData, assetFileGuid);
                if (loadedAsset == null || !(loadedAsset is T))
                {
                    UnloadEbx(importRefr.FileGuid);
                    return default(T);
                }

                LoadedAssetInfo loadedAssetInfo = new LoadedAssetInfo() { LoadedAsset = loadedAsset, EbxAsset = ebxAsset };
                loadedAssets.Add(importRefr, loadedAssetInfo);   
            }

            loadedAssets[importRefr].Increment();
            loadedEbx[importRefr.FileGuid].Increment();

            return loadedAssets[importRefr].LoadedAsset as T;
        }

        public EbxAsset GetEbxAsset(FrostySdk.Ebx.PointerRef pr)
        {
            return GetEbxAsset(pr.External.FileGuid);
        }

        public EbxAsset GetEbxAsset(Guid fileGuid)
        {
            if (!loadedEbx.ContainsKey(fileGuid))
                return null;

            return loadedEbx[fileGuid].EbxAsset;
        }

        public void UpdateAsset(Asset asset)
        {
            EbxImportReference importRefr = new EbxImportReference()
            {
                FileGuid = asset.FileGuid,
                ClassGuid = asset.InstanceGuid
            };

            if (!loadedAssets.ContainsKey(importRefr))
                return;

            EbxAssetEntry entry = App.AssetManager.GetEbxEntry(asset.FileGuid);
            EbxAsset ebxAsset = loadedEbx[importRefr.FileGuid].EbxAsset;

            PushState(entry, loadedEbx[importRefr.FileGuid], false);
            App.AssetManager.ModifyEbx(entry.Name, ebxAsset);
        }

        public void UndoUpdate(Asset asset)
        {
            EbxImportReference importRefr = new EbxImportReference()
            {
                FileGuid = asset.FileGuid,
                ClassGuid = asset.InstanceGuid
            };

            if (!loadedAssets.ContainsKey(importRefr))
                return;

            EbxAssetEntry entry = App.AssetManager.GetEbxEntry(asset.FileGuid);
            EbxAsset ebxAsset = loadedEbx[importRefr.FileGuid].EbxAsset;
            PopState(entry, loadedEbx[importRefr.FileGuid], ebxAsset);
        }

        public void UnloadAsset(Asset asset)
        {
            if (asset == null)
                return;

            EbxImportReference importRefr = new EbxImportReference()
            {
                FileGuid = asset.FileGuid,
                ClassGuid = asset.InstanceGuid
            };

            if (!loadedAssets.ContainsKey(importRefr))
                return;

            loadedAssets[importRefr].Decrement();
            if (loadedAssets[importRefr].RefCount <= 0)
            {
                asset.Dispose();
                loadedAssets.Remove(importRefr);
            }

            UnloadEbx(importRefr.FileGuid);
        }

        private void UnloadEbx(Guid guid)
        {
            if (!loadedEbx.ContainsKey(guid))
                return;

            loadedEbx[guid].Decrement();
            if (loadedEbx[guid].RefCount <= 0)
            {
                loadedEbx.Remove(guid);
            }
        }

        private static Dictionary<Type, Type> assetTypes = new Dictionary<Type, Type>();
        private Asset CreateAsset(FrostySdk.Ebx.Asset assetData, Guid fileGuid)
        {
            if (assetTypes.Count == 0)
            {
                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute<AssetBindingAttribute>() != null))
                {
                    foreach (AssetBindingAttribute attr in type.GetCustomAttributes<AssetBindingAttribute>())
                    {
                        if (assetTypes.ContainsKey(attr.DataType))
                        {
                            Debug.WriteLine($"{attr.DataType},{type}");
                            Debug.Assert(false);
                        }
                        assetTypes.Add(attr.DataType, type);
                    }
                }
            }

            Type assetDataType = assetData.GetType();
            if (assetTypes.ContainsKey(assetDataType))
            {
                Type assetType = assetTypes[assetDataType];
                return (Asset)Activator.CreateInstance(assetType, new object[] { fileGuid, assetData });
            }
            else
            {
                Type tmpType = assetDataType.BaseType;
                while (tmpType != typeof(object))
                {
                    if (assetTypes.ContainsKey(tmpType))
                    {
                        Type assetType = assetTypes[tmpType];
                        return (Asset)Activator.CreateInstance(assetType, new object[] { fileGuid, assetData });
                    }

                    tmpType = tmpType.BaseType;
                }
            }

            // fallback for assets without an explicit class
            return (Asset)Activator.CreateInstance(typeof(GenericAsset<>).MakeGenericType(assetData.GetType()), new object[] { fileGuid, assetData });
        }

        private void PushState(EbxAssetEntry entry, LoadedAssetInfo info, bool initialState)
        {
            if (initialState)
            {
                if (!entry.IsModified)
                    return;
            }

            info.ModificationState.Push(new ModificationState() { IsModified = entry.IsDirectlyModified, IsDirtied = entry.IsDirty });
            App.EditorWindow.DataExplorer.RefreshItems();
        }

        private void PopState(EbxAssetEntry entry, LoadedAssetInfo info, EbxAsset asset)
        {
            ModificationState state = info.ModificationState.Pop();
            if (!state.IsModified)
            {
                entry.ClearModifications();
            }
            else
            {
                App.AssetManager.ModifyEbx(entry.Name, asset);
            }
            entry.IsDirty = state.IsDirtied;
            App.EditorWindow.DataExplorer.RefreshItems();
        }
    }
}
