using Frosty.Core;
using FrostySdk.IO;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.UITtfAsset))]
    public class UITtfAsset : Asset, IAssetData<FrostySdk.Ebx.UITtfAsset>
    {
        public FrostySdk.Ebx.UITtfAsset Data => data as FrostySdk.Ebx.UITtfAsset;
        public FontFamily FontFamily => fontFamily;

        protected FontFamily fontFamily;
        protected PrivateFontCollection pfc;

        public UITtfAsset(Guid fileGuid, FrostySdk.Ebx.UITtfAsset inData)
            : base(fileGuid, inData)
        {
            Load(App.AssetManager.GetRes(App.AssetManager.GetResEntry(Data.FontResource)) as MemoryStream);
        }

        public override void Dispose()
        {
            pfc.Dispose();
            base.Dispose();
        }

        private void Load(MemoryStream stream)
        {
            string fullPath = $"{App.ProfileSettingsPath}/Fonts/{Data.Name}.ttf";
            FileInfo fi = new FileInfo(fullPath);

            if (!fi.Exists)
            {
                if (!fi.Directory.Exists)
                    Directory.CreateDirectory(fi.DirectoryName);

                using (NativeWriter writer = new NativeWriter(new FileStream(fi.FullName, FileMode.Create)))
                {
                    writer.Write(NativeReader.ReadInStream(stream));
                }
            }

            fontFamily = new FontFamily($"{fi.DirectoryName}\\#{GetFontNameFromFile(fi.FullName)}");
        }

        private string GetFontNameFromFile(string path)
        {
            pfc = new PrivateFontCollection();
            pfc.AddFontFile(path);
            return pfc.Families[0].Name;
        }
    }
}
