using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VolumeVectorShapeData))]
	public class VolumeVectorShape : VectorShape, IEntityData<FrostySdk.Ebx.VolumeVectorShapeData>
	{
		public new FrostySdk.Ebx.VolumeVectorShapeData Data => data as FrostySdk.Ebx.VolumeVectorShapeData;
		public override string DisplayName => "VolumeVectorShape";

		public VolumeVectorShape(FrostySdk.Ebx.VolumeVectorShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

