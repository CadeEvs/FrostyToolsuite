using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverAutoGenVolumeShapeData))]
	public class CoverAutoGenVolumeShape : VolumeVectorShape, IEntityData<FrostySdk.Ebx.CoverAutoGenVolumeShapeData>
	{
		public new FrostySdk.Ebx.CoverAutoGenVolumeShapeData Data => data as FrostySdk.Ebx.CoverAutoGenVolumeShapeData;
		public override string DisplayName => "CoverAutoGenVolumeShape";

		public CoverAutoGenVolumeShape(FrostySdk.Ebx.CoverAutoGenVolumeShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

