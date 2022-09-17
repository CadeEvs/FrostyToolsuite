using Frosty.Core.IO;
using Frosty.Core.Mod;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System.Collections.Generic;
using System.IO;
using FrostySdk.Managers.Entries;
using TestPlugin.Resources;

namespace TestPlugin.Handlers
{
    public class DifficultyWeaponTableActionHandler : ICustomActionHandler
    {
        // This is purely for the mod managers action view and has no impact on how the handler actually executes.
        // It tells the mod manager actions view what type of action this handler performs, wether it replaces (Modify)
        // data from one mod with another, or does it merge the two together.
        public HandlerUsage Usage => HandlerUsage.Merge;

        // A mod is comprised of a series of base resources, embedded, ebx, res, and chunks. Embedded are used internally
        // for the icon and images of a mod. Ebx/Res/Chunks are the core resources used for applying data to the game.
        // When you create a custom handler, you need to provide your own resources for your custom handled data. This
        // resource is unique however it is based on one of the three core types.
        private class DifficultyWeaponTableResource : EditorModResource
        {
            // Defines which type of resource this resource is.
            public override ModResourceType Type => ModResourceType.Ebx;

            // Creates a new resource of the specified type and adds its data to the mod manifest.
            public DifficultyWeaponTableResource(EbxAssetEntry entry, FrostyModWriter.Manifest manifest)
                : base(entry)
            {
                // obtain the modified data
                ModifiedResource md = entry.ModifiedEntry.DataObject as ModifiedResource;
                byte[] data = md.Save();

                // store data and details about resource
                name = entry.Name.ToLower();
                sha1 = Utils.GenerateSha1(data);
                resourceIndex = manifest.Add(sha1, data);
                size = data.Length;

                // set the handler hash to the hash of the ebx type name
                handlerHash = Fnv1.HashString(entry.Type.ToLower());
            }
        }

        // The below functions are specific to the editor, it is used to save the modified data to a mod.

        #region -- Editor Specific --

        // This function is for writing resources to the mod file, this is where you would add your custom
        // resources to be written.
        public void SaveToMod(FrostyModWriter writer, AssetEntry entry)
        {
            writer.AddResource(new DifficultyWeaponTableResource(entry as EbxAssetEntry, writer.ResourceManifest));
        }

        #endregion

        // The below functions are specific to the mod manager, it revolves around loading and potentially merging
        // of the data loaded from a mod.

        #region -- Mod Specific --

        // This function is for the mod managers action view, to allow a handler to describe detailed actions performed
        // format of the action string is <ResourceName>;<ResourceType>;<Action> where action can be Modify or Merge
        // and ResourceType can be Ebx,Res,Chunk @todo
        public IEnumerable<string> GetResourceActions(string name, byte[] data)
        {
            var newTable = ModifiedResource.Read(data) as ModifiedDifficultyWeaponTableData;
            List<string> resourceActions = new List<string>();

            foreach (var value in newTable.Values)
            {
                string resourceName = name + " (Row: " + value.Row + "/Col: " + value.Column + ")";
                string resourceType = "ebx";
                string action = "Modify";

                resourceActions.Add(resourceName + ";" + resourceType + ";" + action);
            }

            return resourceActions;
        }

        // This function is invoked when a mod with such a handler is loaded, if a previous mod with a handler for this
        // particular asset was loaded previously, then existing will be populated with that data, allowing this function
        // the chance to merge the two datasets together
        public object Load(object existing, byte[] newData)
        {
            // load the existing modified data (from any previous mods)
            var oldTable = (ModifiedDifficultyWeaponTableData)existing;

            // load the new modified data from the current mod
            var newTable = ModifiedResource.Read(newData) as ModifiedDifficultyWeaponTableData;

            // return the new data if there was no previous data
            if (oldTable == null)
                return newTable;

            // otherwise merge the two together
            foreach (var value in newTable.Values)
            {
                // each change is stored as a row/column/value set, when merged with another mod, each individual
                // row/column change is merged with the previous changes
                oldTable.ModifyValue(value.Row, value.Column, value.Value);
            }

            return oldTable;
        }

        // This function is invoked at the end of the mod loading, to actually modify the existing game data with the end
        // result of the mod loaded data, it also allows for a handler to add new Resources to be replaced.
        // ie. an Ebx handler might want to add a new Chunk resource that it is dependent on.
        public void Modify(AssetEntry origEntry, AssetManager am, RuntimeResources runtimeResources, object data, out byte[] outData)
        {
            // obtain the modified data that has been loaded and merged from the mods
            ModifiedDifficultyWeaponTableData modifiedData = data as ModifiedDifficultyWeaponTableData;

            // load the original game ebx asset
            EbxAsset asset = am.GetEbx(am.GetEbxEntry(origEntry.Name));
            dynamic rootTable = asset.RootObject;

            // replace data from the game ebx data with the modified data
            foreach (var value in modifiedData.Values)
            {
                rootTable.WeaponTable[value.Row].Values[value.Column].Value = value.Value;
            }

            // write out the new ebx data
            using (EbxBaseWriter writer = EbxBaseWriter.CreateWriter(new MemoryStream()))
            {
                writer.WriteAsset(asset);
                origEntry.OriginalSize = writer.Length;
                outData = Utils.CompressFile(writer.ToByteArray());
            }

            // update relevant asset entry values
            origEntry.Size = outData.Length;
            origEntry.Sha1 = Utils.GenerateSha1(outData);
        }

        #endregion
    }
}
