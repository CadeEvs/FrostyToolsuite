using Frosty.Core.Attributes;
using Frosty.Core.Controls;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using FrostySdk.Managers.Entries;

namespace Frosty.Core
{
    /// <summary>
    /// Represents an extension and description pair that is combined to make a valid export filter
    /// </summary>
    public struct AssetExportType
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        /// <returns>The extension.</returns>
        public string Extension { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <returns>The description.</returns>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a valid filter string using the combination of the description and extension variables
        /// </summary>
        /// <returns>A valid filter string</returns>
        public string FilterString => Description + " (*." + Extension + ")|*." + Extension;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetExportType"/> struct with the extension and description.
        /// </summary>
        public AssetExportType(string ext, string desc)
        {
            Extension = ext;
            Description = desc;
        }
    }

    /// <summary>
    /// Represents an extension and description pair that is combined to make a valid import filter
    /// </summary>
    public struct AssetImportType
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        /// <returns>The extension.</returns>
        public string Extension { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <returns>The description.</returns>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a valid filter string using the combination of the description and extension variables
        /// </summary>
        /// <returns>A valid filter string</returns>
        public string FilterString => Description + " (*." + Extension + ")|*." + Extension;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetImportType"/> struct with the extension and description.
        /// </summary>
        public AssetImportType(string ext, string desc)
        {
            Extension = ext;
            Description = desc;
        }
    }

    /// <summary>
    /// Describes editor related functionality for a specified asset type. Classes that derive from this class
    /// can set the icon that will be used, override the existing editor and define import/export methods, used in conjunction with the
    /// <see cref="RegisterAssetDefinitionAttribute"/> class to register asset types to specific <see cref="AssetDefinition"/> derivatives.
    /// </summary>
    public class AssetDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetDefinition"/> class.
        /// </summary>
        public AssetDefinition()
        {
        }

        /// <summary>
        /// When implemented in a derived class, allows the asset definition to provide a list of valid types for exporting an asset.
        /// </summary>
        /// <param name="exportTypes">A list of <see cref="AssetExportType"/> to be populated.</param>
        public virtual void GetSupportedExportTypes(List<AssetExportType> exportTypes)
        {
            exportTypes.Add(new AssetExportType("xml", "XML File"));
            exportTypes.Add(new AssetExportType("bin", "Binary File"));
        }

        /// <summary>
        /// When impemented in a derived class, allows the asset definition to provide a list of valid types for importing an asset.
        /// </summary>
        /// <param name="importTypes">A list of <see cref="AssetImportType"/> to be populated.</param>
        public virtual void GetSupportedImportTypes(List<AssetImportType> importTypes)
        {
            importTypes.Add(new AssetImportType("bin", "Binary File"));
        }

        /// <summary>
        /// When implemented in a derived class, returns a custom editor that is used to edit the asset defined by the asset definition. Must inherit from <see cref="FrostyAssetEditor"/>.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger"/> that represents the logging interface to use.</param>
        /// <returns>A custom editor that is used to edit the asset defined by the asset definition. Must inherit from <see cref="FrostyAssetEditor"/>.</returns>
        public virtual FrostyAssetEditor GetEditor(ILogger logger)
        {
            return null;
        }

        /// <summary>
        /// When implemented in a derived class, returns an <see cref="ImageSource"/> that represents a generic icon for this asset type
        /// </summary>
        /// <returns>An <see cref="ImageSource"/> that represents a generic icon for this asset type</returns>
        public virtual ImageSource GetIcon()
        {
            return null;
        }

        /// <summary>
        /// When implemented in a derived class, returns an <see cref="ImageSource"/> that represents an icon for the specified asset, the width and height parameters define a best size to use
        /// for the resulting icon (However these values can be PositiveInfinity meaning no size information available).
        /// </summary>
        /// <param name="entry">The <see cref="AssetEntry"/> this icon will represent.</param>
        /// <param name="width">A double representing the estimated width to use (can be PositiveInfinity).</param>
        /// <param name="height">A double representing the estimated height to use (can be PositiveInfinity).</param>
        /// <returns>An <see cref="ImageSource"/> that represents an icon for the specified asset.</returns>
        public virtual ImageSource GetIcon(AssetEntry entry, double width = double.PositiveInfinity, double height = double.PositiveInfinity)
        {
            return GetIcon();
        }

        /// <summary>
        /// When implemented in a derived class, performs the actual export of the asset, providing the filter type and path to export to.
        /// </summary>
        /// <param name="entry">The <see cref="AssetEntry"/> to export.</param>
        /// <param name="path">A string representing the path and filename to export to.</param>
        /// <param name="filterType">A string representing the chosen filter type to export as.</param>
        /// <returns>True if export was successful, False otherwise.</returns>
        public virtual bool Export(EbxAssetEntry entry, string path, string filterType)
        {
            if (filterType == "xml")
            {
                ExportToXml(entry, path);
                return true;
            }
            
            if (filterType == "bin")
            {
                ExportToBin(entry, path);
                return true;
            }
            return false;
        }

        /// <summary>
        /// When implemented in a derived class, performs the actual import of the asset, providing the filter type and path to import from.
        /// </summary>
        /// <param name="entry">The <see cref="AssetEntry"/> to import the data to.</param>
        /// <param name="path">A string representing the path and filename to import from.</param>
        /// <param name="filterType">A string representing the chosen filter type to import as.</param>
        /// <returns>True if import was successful, False otherwise.</returns>
        public virtual bool Import(EbxAssetEntry entry, string path, string filterType)
        {
            if (filterType == "bin")
            {
                ImportFromBin(entry, path);
                return true;
            }
            return false;
        }

        // exports the asset to xml
        private void ExportToXml(EbxAssetEntry entry, string path)
        {
            EbxAsset asset = App.AssetManager.GetEbx(entry);
            using (EbxXmlWriter writer = new EbxXmlWriter(new FileStream(path, FileMode.Create), App.AssetManager))
                writer.WriteObjects(asset.Objects);
        }

        // exports the asset as raw ebx
        private void ExportToBin(EbxAssetEntry entry, string path)
        {
            if (App.PluginManager.GetCustomHandler(entry.Type) != null)
            {
                // @todo: throw some kind of error
                return;
            }

            if (ProfilesLibrary.EnableExecution && entry.HasModifiedData)
            {
                EbxWriteFlags flags = EbxWriteFlags.None;
                if (ProfilesLibrary.EbxVersion == 6)
                {
                    flags |= EbxWriteFlags.DoNotSort;
                }

                using (EbxBaseWriter writer = EbxBaseWriter.CreateWriter(new MemoryStream(), flags))
                {
                    writer.WriteAsset(App.AssetManager.GetEbx(entry));
                    using (NativeWriter fileWriter = new NativeWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
                        fileWriter.Write(writer.ToByteArray());
                }
            }
            else
            {
                Stream asset = App.AssetManager.GetEbxStream(entry);
                using (NativeWriter writer = new NativeWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
                    writer.Write(NativeReader.ReadInStream(asset));
            }
        }

        // imports the asset from a raw ebx bin
        private void ImportFromBin(EbxAssetEntry entry, string path)
        {
            if (App.PluginManager.GetCustomHandler(entry.Type) != null)
            {
                // @todo: throw some kind of error
                return;
            }

            using (EbxReader reader = EbxReader.CreateReader(new FileStream(path, FileMode.Open, FileAccess.Read), App.FileSystemManager, true))
            {
                var asset = reader.ReadAsset<EbxAsset>();
                App.AssetManager.ModifyEbx(entry.Name, asset);
            }
        }
    }
}
