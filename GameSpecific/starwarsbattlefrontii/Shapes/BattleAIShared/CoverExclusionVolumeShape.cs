using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverExclusionVolumeShapeData))]
	public class CoverExclusionVolumeShape : VolumeVectorShape, IEntityData<FrostySdk.Ebx.CoverExclusionVolumeShapeData>
	{
		public new FrostySdk.Ebx.CoverExclusionVolumeShapeData Data => data as FrostySdk.Ebx.CoverExclusionVolumeShapeData;
		public override string DisplayName => "CoverExclusionVolumeShape";

		public CoverExclusionVolumeShape(FrostySdk.Ebx.CoverExclusionVolumeShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

