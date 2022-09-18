using Frosty.Core;
using FrostySdk;
using FrostySdk.IO;
using LevelEditorPlugin.Editors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrostySdk.Ebx;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;

namespace LevelEditorPlugin.Extensions
{
    public class BlueprintAnalysisExtension : MenuExtension
    {
        public override string TopLevelMenuName => "Developer";
        public override string MenuItemName => "Blueprint Analysis";
        public override RelayCommand MenuItemClicked => new RelayCommand((o) =>
        {
            using (NativeWriter writer = new NativeWriter(new FileStream("BlueprintAnalysis.csv", FileMode.Create)))
            {
                foreach (EbxAssetEntry entry in App.AssetManager.EnumerateEbx())
                {
                    if (TypeLibrary.IsSubClassOf(entry.Type, "Blueprint"))
                    {
                        EbxAsset asset = App.AssetManager.GetEbx(entry);
                        Blueprint blueprint = asset.RootObject as FrostySdk.Ebx.Blueprint;

                        if (blueprint is FrostySdk.Ebx.LogicPrefabBlueprint)
                        {
                            PrefabBlueprint prefabBlueprint = blueprint as FrostySdk.Ebx.PrefabBlueprint;
                            foreach (PointerRef objRef in prefabBlueprint.Objects)
                            {
                                if (objRef.Internal != null)
                                {
                                    GameObjectData entityData = (FrostySdk.Ebx.GameObjectData)objRef.Internal;
                                    int flags = (int)(entityData.Flags >> 25);

                                    int tmp = (int)(entityData.Flags & 0x1FFFFFF);
                                    uint tmp2 = 0;

                                    if (entityData.__InstanceGuid.IsExported)
                                    {
                                        byte[] array = entityData.__InstanceGuid.ExportedGuid.ToByteArray();
                                        tmp2 = (uint)((int)(array[3] & 0x01) << 24 | (int)array[2] << 16 | (int)array[1] << 8 | (int)array[0]);
                                    }

                                    int inputPropertyCount = 0;
                                    int outputPropertyCount = 0;
                                    int inputEventCount = 0;
                                    int outputEventCount = 0;
                                    int inputLinkCount = 0;
                                    int outputLinkCount = 0;
                                    FrostySdk.Ebx.Realm realm = FrostySdk.Ebx.Realm.Realm_None;

                                    try
                                    {
                                        realm = ((dynamic)entityData).Realm;
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    foreach (PropertyConnection propConnection in prefabBlueprint.PropertyConnections)
                                    {
                                        if (propConnection.Source.Internal == entityData) inputPropertyCount = 1;
                                        if (propConnection.Target.Internal == entityData) outputPropertyCount = 1;
                                        if (propConnection.Source.External.ClassGuid == entityData.__InstanceGuid.ExportedGuid) inputPropertyCount = 1;
                                        if (propConnection.Target.External.ClassGuid == entityData.__InstanceGuid.ExportedGuid) outputPropertyCount = 1;
                                    }
                                    foreach (EventConnection eventConnection in prefabBlueprint.EventConnections)
                                    {
                                        if (eventConnection.Source.Internal == entityData) inputEventCount = 1;
                                        if (eventConnection.Target.Internal == entityData) outputEventCount = 1;
                                        if (eventConnection.Source.External.ClassGuid == entityData.__InstanceGuid.ExportedGuid) inputEventCount = 1;
                                        if (eventConnection.Target.External.ClassGuid == entityData.__InstanceGuid.ExportedGuid) outputEventCount = 1;
                                    }
                                    foreach (LinkConnection linkConnection in prefabBlueprint.LinkConnections)
                                    {
                                        if (linkConnection.Source.Internal == entityData) inputLinkCount = 1;
                                        if (linkConnection.Target.Internal == entityData) outputLinkCount = 1;
                                        if (linkConnection.Source.External.ClassGuid == entityData.__InstanceGuid.ExportedGuid) inputLinkCount = 1;
                                        if (linkConnection.Target.External.ClassGuid == entityData.__InstanceGuid.ExportedGuid) outputLinkCount = 1;
                                    }

                                    FrostySdk.Ebx.Realm propRealm = FrostySdk.Ebx.Realm.Realm_None;
                                    FrostySdk.Ebx.Realm eventRealm = FrostySdk.Ebx.Realm.Realm_None;

                                    if ((flags & 0x03) == 0x03) propRealm = FrostySdk.Ebx.Realm.Realm_ClientAndServer;
                                    else if ((flags & 0x01) == 0x01) propRealm = FrostySdk.Ebx.Realm.Realm_Client;
                                    else if ((flags & 0x02) == 0x02) propRealm = FrostySdk.Ebx.Realm.Realm_Server;

                                    if ((flags & 0x0c) == 0x0c) eventRealm = FrostySdk.Ebx.Realm.Realm_ClientAndServer;
                                    else if ((flags & 0x04) == 0x04) eventRealm = FrostySdk.Ebx.Realm.Realm_Client;
                                    else if ((flags & 0x08) == 0x08) eventRealm = FrostySdk.Ebx.Realm.Realm_Server;

                                    string str = (realm != FrostySdk.Ebx.Realm.Realm_None) ? realm.ToString() : "";
                                    string propStr = (propRealm != FrostySdk.Ebx.Realm.Realm_None) ? propRealm.ToString() : "";
                                    string eventStr = (eventRealm != FrostySdk.Ebx.Realm.Realm_None) ? eventRealm.ToString() : "";

                                    writer.WriteLine($"{entityData.GetType().Name},\"{flags.ToString("x8")}h\",\"{tmp.ToString("x8")}h\",\"{tmp2.ToString("x8")}h\",{tmp2==tmp},{inputPropertyCount},{outputPropertyCount},{inputEventCount},{outputEventCount},{inputLinkCount},{outputLinkCount},{str},{propStr},{eventStr},{entry.Filename}");
                                }
                            }
                        }
                    }
                }
            }
        });
    }
}
