using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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

        private Dictionary<EbxImportReference, LoadedAssetInfo> m_loadedAssets = new Dictionary<EbxImportReference, LoadedAssetInfo>();
        private Dictionary<Guid, LoadedAssetInfo> m_loadedEbx = new Dictionary<Guid, LoadedAssetInfo>();

        private static Dictionary<Type, Type> m_assetTypes = new Dictionary<Type, Type>();
        
        public T LoadAsset<T>(Guid assetFileGuid) where T : Asset
        {
            EbxAssetEntry entry = App.AssetManager.GetEbxEntry(assetFileGuid);
            if (entry == null)
                return default(T);
            return LoadAsset<T>(new EbxImportReference() { FileGuid = entry.Guid, ClassGuid = Guid.Empty });
        }

        public T LoadAsset<T>(EbxImportReference importRefr) where T : Asset
        {
            if (!m_loadedAssets.ContainsKey(importRefr))
            {
                EbxAsset ebxAsset = (m_loadedEbx.ContainsKey(importRefr.FileGuid))
                    ? m_loadedEbx[importRefr.FileGuid].EbxAsset
                    : App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(importRefr.FileGuid));

                if (importRefr.ClassGuid == Guid.Empty)
                {
                    importRefr.ClassGuid = ebxAsset.RootInstanceGuid;
                }

                if (!m_loadedAssets.ContainsKey(importRefr))
                {
                    if (!m_loadedEbx.ContainsKey(importRefr.FileGuid))
                    {
                        EbxAssetEntry entry = App.AssetManager.GetEbxEntry(importRefr.FileGuid);
                        m_loadedEbx.Add(importRefr.FileGuid, new LoadedAssetInfo() { EbxAsset = ebxAsset });
                        PushState(entry, m_loadedEbx[importRefr.FileGuid], true);
                    }

                    object obj = ebxAsset.GetObject(importRefr.ClassGuid);
                    Asset loadedAsset = CreateAsset(obj as FrostySdk.Ebx.Asset, importRefr.FileGuid);

                    if (loadedAsset == null || !(loadedAsset is T))
                    {
                        UnloadEbx(importRefr.FileGuid);
                        return default(T);
                    }

                    m_loadedAssets.Add(importRefr, new LoadedAssetInfo() { LoadedAsset = loadedAsset });
                }
            }

            m_loadedAssets[importRefr].Increment();
            m_loadedEbx[importRefr.FileGuid].Increment();

            return m_loadedAssets[importRefr].LoadedAsset as T;
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

            if (!m_loadedAssets.ContainsKey(importRefr))
            {
                if (!m_loadedEbx.ContainsKey(importRefr.FileGuid))
                {
                    Debug.Assert(ebxAsset != null);
                    EbxAssetEntry entry = App.AssetManager.GetEbxEntry(ebxAsset.FileGuid);
                    m_loadedEbx.Add(importRefr.FileGuid, new LoadedAssetInfo() { EbxAsset = ebxAsset });
                    PushState(entry, m_loadedEbx[importRefr.FileGuid], true);
                }

                Asset loadedAsset = CreateAsset(assetData, assetFileGuid);
                if (loadedAsset == null || !(loadedAsset is T))
                {
                    UnloadEbx(importRefr.FileGuid);
                    return default(T);
                }

                LoadedAssetInfo loadedAssetInfo = new LoadedAssetInfo() { LoadedAsset = loadedAsset, EbxAsset = ebxAsset };
                m_loadedAssets.Add(importRefr, loadedAssetInfo);   
            }

            m_loadedAssets[importRefr].Increment();
            m_loadedEbx[importRefr.FileGuid].Increment();

            return m_loadedAssets[importRefr].LoadedAsset as T;
        }

        public EbxAsset GetEbxAsset(FrostySdk.Ebx.PointerRef pr)
        {
            return GetEbxAsset(pr.External.FileGuid);
        }

        public EbxAsset GetEbxAsset(Guid fileGuid)
        {
            if (!m_loadedEbx.ContainsKey(fileGuid))
            {
                return null;
            }

            return m_loadedEbx[fileGuid].EbxAsset;
        }

        public bool IsAssetModified(Guid fileGuid)
        {
            EbxAssetEntry entry = App.AssetManager.GetEbxEntry(fileGuid);

            return entry.IsModified;
        }
        
        public void UpdateAsset(Asset asset)
        {
            EbxImportReference importRefr = new EbxImportReference()
            {
                FileGuid = asset.FileGuid,
                ClassGuid = asset.InstanceGuid
            };

            if (!m_loadedAssets.ContainsKey(importRefr))
            {
                return;
            }

            EbxAssetEntry entry = App.AssetManager.GetEbxEntry(asset.FileGuid);
            EbxAsset ebxAsset = m_loadedEbx[importRefr.FileGuid].EbxAsset;

            PushState(entry, m_loadedEbx[importRefr.FileGuid], false);
            App.AssetManager.ModifyEbx(entry.Name, ebxAsset);
        }

        public void UndoUpdate(Asset asset)
        {
            EbxImportReference importRefr = new EbxImportReference()
            {
                FileGuid = asset.FileGuid,
                ClassGuid = asset.InstanceGuid
            };

            if (!m_loadedAssets.ContainsKey(importRefr))
                return;

            EbxAssetEntry entry = App.AssetManager.GetEbxEntry(asset.FileGuid);
            EbxAsset ebxAsset = m_loadedEbx[importRefr.FileGuid].EbxAsset;
            PopState(entry, m_loadedEbx[importRefr.FileGuid], ebxAsset);
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

            if (!m_loadedAssets.ContainsKey(importRefr))
                return;

            m_loadedAssets[importRefr].Decrement();
            if (m_loadedAssets[importRefr].RefCount <= 0)
            {
                asset.Dispose();
                m_loadedAssets.Remove(importRefr);
            }

            UnloadEbx(importRefr.FileGuid);
        }

        private void UnloadEbx(Guid guid)
        {
            if (!m_loadedEbx.ContainsKey(guid))
                return;

            m_loadedEbx[guid].Decrement();
            if (m_loadedEbx[guid].RefCount <= 0)
            {
                m_loadedEbx.Remove(guid);
            }
        }
        
        private Asset CreateAsset(FrostySdk.Ebx.Asset assetData, Guid fileGuid)
        {
            if (m_assetTypes.Count == 0)
            {
                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute<AssetBindingAttribute>() != null))
                {
                    foreach (AssetBindingAttribute attr in type.GetCustomAttributes<AssetBindingAttribute>())
                    {
                        if (m_assetTypes.ContainsKey(attr.DataType))
                        {
                            Debug.WriteLine($"{attr.DataType},{type}");
                            Debug.Assert(false);
                        }
                        m_assetTypes.Add(attr.DataType, type);
                    }
                }
            }

            Type assetDataType = assetData.GetType();
            if (m_assetTypes.ContainsKey(assetDataType))
            {
                Type assetType = m_assetTypes[assetDataType];
                return (Asset)Activator.CreateInstance(assetType, new object[] { fileGuid, assetData });
            }
            else
            {
                Type tmpType = assetDataType.BaseType;
                while (tmpType != typeof(object))
                {
                    if (m_assetTypes.ContainsKey(tmpType))
                    {
                        Type assetType = m_assetTypes[tmpType];
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
