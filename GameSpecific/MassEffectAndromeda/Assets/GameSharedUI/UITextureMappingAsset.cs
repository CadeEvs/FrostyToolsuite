using Frosty.Core;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.UITextureMappingAsset))]
    public class UITextureMappingAsset : Asset, IAssetData<FrostySdk.Ebx.UITextureMappingAsset>
    {
        public FrostySdk.Ebx.UITextureMappingAsset Data => data as FrostySdk.Ebx.UITextureMappingAsset;

        protected Dictionary<ulong, BitmapImage> textureAssets = new Dictionary<ulong, BitmapImage>();
        protected Dictionary<int, ulong> textureIds = new Dictionary<int, ulong>();

        public UITextureMappingAsset(Guid fileGuid, FrostySdk.Ebx.UITextureMappingAsset inData)
            : base(fileGuid, inData)
        {
            foreach (var output in Data.Output)
            {
                if (!textureAssets.ContainsKey(output.TextureRef))
                {
                    var texture = App.AssetManager.GetResAs<Texture>(App.AssetManager.GetResEntry(output.TextureRef));
                    var bitmap = TextureCreator.CreatePNG(texture);

                    textureAssets.Add(output.TextureRef, bitmap);
                }
                textureIds.Add(output.Id, output.TextureRef);
            }
        }

        public BitmapImage GetTexture(int id)
        {
            if (!textureIds.ContainsKey(id))
                return null;

            return textureAssets[textureIds[id]];
        }

        public Point4D GetUVRect(int id)
        {
            var result = Data.Output.FirstOrDefault(t => t.Id == id);
            return new Point4D(result.UvRect.x, result.UvRect.y, result.UvRect.z, result.UvRect.w);
        }
    }
}
