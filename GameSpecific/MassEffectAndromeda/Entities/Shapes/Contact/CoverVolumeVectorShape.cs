using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverVolumeVectorShapeData))]
	public class CoverVolumeVectorShape : VolumeVectorShape, IEntityData<FrostySdk.Ebx.CoverVolumeVectorShapeData>
	{
		public new FrostySdk.Ebx.CoverVolumeVectorShapeData Data => data as FrostySdk.Ebx.CoverVolumeVectorShapeData;
		public override string DisplayName => "CoverVolumeVectorShape";

		public CoverVolumeVectorShape(FrostySdk.Ebx.CoverVolumeVectorShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

