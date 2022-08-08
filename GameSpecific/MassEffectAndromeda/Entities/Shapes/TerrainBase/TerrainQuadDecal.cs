using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TerrainQuadDecalData))]
	public class TerrainQuadDecal : VisualVectorShape, IEntityData<FrostySdk.Ebx.TerrainQuadDecalData>
	{
		public new FrostySdk.Ebx.TerrainQuadDecalData Data => data as FrostySdk.Ebx.TerrainQuadDecalData;
		public override string DisplayName => "TerrainQuadDecal";

		public TerrainQuadDecal(FrostySdk.Ebx.TerrainQuadDecalData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

