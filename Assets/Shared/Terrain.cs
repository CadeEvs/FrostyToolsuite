using Frosty.Core;
using Frosty.Core.Viewport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.TerrainData))]
    public class Terrain : Asset, IAssetData<FrostySdk.Ebx.TerrainData>
    {
        public FrostySdk.Ebx.TerrainData Data => data as FrostySdk.Ebx.TerrainData;
        public Render.TerrainRenderable TerrainData { get; private set; }
        public int MaxLayerCount { get; private set; }

        private Resources.TerrainStreamingTree terrainStreamingTree;
        private Resources.TerrainLayerCombinations terrainLayerCombinations;
        //private Resources.TerrainDecals terrainDecals;
        private Resources.VisualTerrain visualTerrain;

        public Terrain(Guid fileGuid, FrostySdk.Ebx.TerrainData inData)
            : base(fileGuid, inData)
        {
            terrainStreamingTree = App.AssetManager.GetResAs<Resources.TerrainStreamingTree>(App.AssetManager.GetResEntry(Data.TerrainStreamingTreeResource));
            terrainLayerCombinations = App.AssetManager.GetResAs<Resources.TerrainLayerCombinations>(App.AssetManager.GetResEntry(Data.TerrainLayerCombinationsResource));
            visualTerrain = App.AssetManager.GetResAs<Resources.VisualTerrain>(App.AssetManager.GetResEntry(Data.VisualResource));

            MaxLayerCount = (int)(terrainStreamingTree.GetRasterTree(Resources.RasterTreeTypes.TerrainMaskTreeType) as Resources.TerrainMaskTree).maxLayerCount;
        }

        public void LoadResource(RenderCreateState state)
        {
            if (TerrainData == null)
            {
                TerrainData = new Render.TerrainRenderable(state, terrainStreamingTree);
            }
        }

        public override void Dispose()
        {
            TerrainData.Dispose();
        }
    }
}
