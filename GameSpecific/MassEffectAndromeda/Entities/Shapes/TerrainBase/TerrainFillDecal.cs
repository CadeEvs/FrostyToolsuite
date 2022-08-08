using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TerrainFillDecalData))]
	public class TerrainFillDecal : VisualVectorShape, IEntityData<FrostySdk.Ebx.TerrainFillDecalData>
	{
		public new FrostySdk.Ebx.TerrainFillDecalData Data => data as FrostySdk.Ebx.TerrainFillDecalData;
		public override string DisplayName => "TerrainFillDecal";

		public TerrainFillDecal(FrostySdk.Ebx.TerrainFillDecalData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

