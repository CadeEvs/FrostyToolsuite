using BiowareLocalizationPlugin.LocalizedResources;
using Frosty.Core;
using Frosty.Core.IO;
using Frosty.Core.Mod;
using Frosty.Hash;
using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace BiowareLocalizationPlugin
{
    public class BiowareLocalizationCustomActionHandler : ICustomActionHandler
    {

        public HandlerUsage Usage => HandlerUsage.Merge;

        // A mod is comprised of a series of base resources, embedded, ebx, res, and chunks. Embedded are used internally
        // for the icon and images of a mod. Ebx/Res/Chunks are the core resources used for applying data to the game.
        // When you create a custom handler, you need to provide your own resources for your custom handled data. This
        // resource is unique however it is based on one of the three core types.
        private class BiowareLocalizationModResource : EditorModResource
        {

            // Defines which type of resource this resource is.
            public override ModResourceType Type => ModResourceType.Res;


            // The resType is vital to be kept (its always LocalizedStringResource, but whatever)
            private readonly uint m_resType;

            // these other two fields may have to be written to the mod as well
            private readonly ulong m_resRid;
            private readonly byte[] m_resMeta;


            public BiowareLocalizationModResource(ResAssetEntry inEntry, FrostyModWriter.Manifest inManifest) : base(inEntry)
            {

                // This constructor does the exact same thing as the ones in the TestPlugin

                // obtain the modified data
                ModifiedLocalizationResource md = inEntry.ModifiedEntry.DataObject as ModifiedLocalizationResource;
                byte[] data = md.Save();

                // store data and details about resource
                name = inEntry.Name.ToLower();
                sha1 = Utils.GenerateSha1(data);
                resourceIndex = inManifest.Add(sha1, data);
                size = data.Length;

                // set the handler hash to the hash of the res type name
                handlerHash = Fnv1.HashString(inEntry.Type.ToLower());

                m_resType = inEntry.ResType;
                m_resRid = inEntry.ResRid;
                m_resMeta = inEntry.ResMeta;
            }

            /// <summary>
            /// This method is calles when writing the mod. For Res Types it is vital that some additional information is persisted that is not written by the base method.
            /// Mainly that is the ResourceType as uint
            /// Additional data that is read, but I'm not sure whether it is actually necessary:
            /// <ul>
            /// <li>ResRid as ulong (not sure if this is really necessary, i.e., actually read)
            /// <li>resMeta length
            /// <li>resMeta as byte array
            /// </ul>
            /// </summary>
            /// <param name="writer"></param>
            public override void Write(NativeWriter writer)
            {
                base.Write(writer);

                // write the required res type:
                writer.Write(m_resType);

                writer.Write(m_resRid);
                writer.Write((m_resMeta != null) ? m_resMeta.Length : 0);
                if (m_resMeta != null)
                {
                    writer.Write(m_resMeta);
                }
            }
        }

        #region -- Editor Specific --

        // This function is for writing resources to the mod file, this is where you would add your custom
        // resources to be written.
        public void SaveToMod(FrostyModWriter writer, AssetEntry entry)
        {
            writer.AddResource(new BiowareLocalizationModResource(entry as ResAssetEntry, writer.ResourceManifest));
        }
        #endregion

        #region -- Mod Specific --

        // This function is for the mod managers action view, to allow a handler to describe detailed actions performed
        // format of the action string is <ResourceName>;<ResourceType>;<Action> where action can be Modify or Merge (or Add!)
        // and ResourceType can be Ebx,Res,Chunk
        public IEnumerable<string> GetResourceActions(string name, byte[] data)
        {

            if( !Config.Get(BiowareLocalizationPluginOptions.SHOW_INDIVIDUAL_TEXTIDS_OPTION_NAME, false, ConfigScope.Global))
            {
                return new List<string>();
            }

            ModifiedLocalizationResource modified = ModifiedResource.Read(data) as ModifiedLocalizationResource;

            List<uint> textIds = new List<uint>(modified.AlteredTexts.Keys);
            textIds.Sort();

            List<string> resourceActions = new List<string>(textIds.Count);
            foreach (uint textId in textIds)
            {
                string resourceName = new StringBuilder(name).Append(" (0x").Append(textId.ToString("X8")).Append(')').ToString();
                string resourceType = "res";
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

            ModifiedLocalizationResource edited = (ModifiedLocalizationResource)existing;
            ModifiedLocalizationResource newTexts = (ModifiedLocalizationResource) ModifiedResource.Read(newData);

            if(edited == null)
            {
                return newTexts;
            }

            edited.Merge(newTexts);

            return edited;
        }

        // This function is invoked at the end of the mod loading, to actually modify the existing game data with the end
        // result of the mod loaded data, it also allows for a handler to add new Resources to be replaced.
        // ie. an Ebx handler might want to add a new Chunk resource that it is dependent on.
        public void Modify(AssetEntry origEntry, AssetManager am, RuntimeResources runtimeResources, object data, out byte[] outData)
        {

            // no idea what frosty does if the resource does not exist in the local game, so first check for null:
            if(origEntry == null)
            {
                outData = System.Array.Empty<byte>();
                return;
            }

            // load the original resource
            ResAssetEntry originalResAsset = am.GetResEntry(origEntry.Name);
            ModifiedLocalizationResource modified = data as ModifiedLocalizationResource;
            LocalizedStringResource resource = am.GetResAs<LocalizedStringResource>(originalResAsset, modified);

            // read about some weird null reference exception in here, so _maybe_ it was the resource?
            if(resource == null)
            {
                throw new ArgumentNullException("resource", string.Format("Resource in BwLocalizationHandler Modify(...) is null after GetResAs call for <{0}>!", origEntry.Name));
            }

            byte[] uncompressedData = resource.SaveBytes();
            outData = Utils.CompressFile(uncompressedData);

            // update the metadata
            byte[] alteredMetaData = resource.ResourceMeta;
            ((ResAssetEntry)origEntry).ResMeta = alteredMetaData;

            // update relevant asset entry values
            origEntry.OriginalSize = uncompressedData.Length;
            origEntry.Size = outData.Length;
            origEntry.Sha1 = Utils.GenerateSha1(outData);
        }
        #endregion
    }
}
