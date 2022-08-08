using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MantleVolumeVectorShapeData))]
	public class MantleVolumeVectorShape : VolumeVectorShape, IEntityData<FrostySdk.Ebx.MantleVolumeVectorShapeData>
	{
		public new FrostySdk.Ebx.MantleVolumeVectorShapeData Data => data as FrostySdk.Ebx.MantleVolumeVectorShapeData;
		public override string DisplayName => "MantleVolumeVectorShape";

		public MantleVolumeVectorShape(FrostySdk.Ebx.MantleVolumeVectorShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

