using Frosty.Core.IO;
using Frosty.Core.Mod;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System.Collections.Generic;
using System.IO;
using TestPlugin.Resources;

namespace TestPlugin.Handlers
{
    public class SvgImageCustomActionHandler : ICustomActionHandler
    {
        // This is purely for the mod managers action view and has no impact on how the handler actually executes.
        // It tells the mod manager actions view what type of action this handler performs, wether it replaces (Modify)
        // data from one mod with another, or does it merge the two together.
        public HandlerUsage Usage => HandlerUsage.Modify;

        // A mod is comprised of a series of base resources, embedded, ebx, res, and chunks. Embedded are used internally
        // for the icon and images of a mod. Ebx/Res/Chunks are the core resources used for applying data to the game.
        // When you create a custom handler, you need to provide your own resources for your custom handled data. This
        // resource is unique however it is based on one of the three core types.
        private class SvgImageResource : EditorModResource
        {
            // Defines which type of resource this resource is.
            public override ModResourceType Type => ModResourceType.Ebx;

            // Creates a new resource of the specified type and adds its data to the mod manifest.
            public SvgImageResource(EbxAssetEntry entry, FrostyModWriter.Manifest manifest)
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

        public void SaveToMod(FrostyModWriter writer, AssetEntry entry)
        {
            writer.AddResource(new SvgImageResource(entry as EbxAssetEntry, writer.ResourceManifest));
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
            return new List<string>();
        }

        // This function is invoked when a mod with such a handler is loaded, if a previous mod with a handler for this
        // particular asset was loaded previously, then existing will be populated with that data, allowing this function
        // the chance to merge the two datasets together
        public object Load(object existing, byte[] newData)
        {
            // just return the new modified data (completely replacing the existing)
            return ModifiedResource.Read(newData) as ModifiedSvgImageAsset;
        }

        // This function is invoked at the end of the mod loading, to actually modify the existing game data with the end
        // result of the mod loaded data, it also allows for a handler to add new Resources to be replaced.
        // ie. an Ebx handler might want to add a new Chunk resource that it is dependent on.
        public void Modify(AssetEntry origEntry, AssetManager am, RuntimeResources runtimeResources, object data, out byte[] outData)
        {
            // obtain the modified data that has been loaded and merged from the mods
            ModifiedSvgImageAsset modifiedSvg = data as ModifiedSvgImageAsset;

            // load the original game ebx asset
            EbxAsset asset = am.GetEbx(am.GetEbxEntry(origEntry.Name));
            dynamic svgData = asset.RootObject;

            // load the original SVG resource
            ResAssetEntry origResEntry = am.GetResEntry(svgData.Resource);

            // create a new Resource and populate based on the original
            ResAssetEntry newResEntry = new ResAssetEntry();
            byte[] buf = modifiedSvg.Resource.SaveBytes();

            newResEntry.Name = origResEntry.Name;
            newResEntry.Size = buf.Length;
            newResEntry.ResRid = origResEntry.ResRid;
            newResEntry.ResType = origResEntry.ResType;
            newResEntry.ResMeta = origResEntry.ResMeta;
            newResEntry.Bundles.AddRange(origEntry.Bundles);

            buf = Utils.CompressFile(buf);
            newResEntry.Sha1 = Utils.GenerateSha1(buf);

            // add the new resource to be replaced
            runtimeResources.AddResource(new RuntimeResResource(newResEntry), buf);

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
