using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScanlineVolumeShapeData))]
	public class ScanlineVolumeShape : VolumeVectorShape, IEntityData<FrostySdk.Ebx.ScanlineVolumeShapeData>
	{
		public new FrostySdk.Ebx.ScanlineVolumeShapeData Data => data as FrostySdk.Ebx.ScanlineVolumeShapeData;
		public override string DisplayName => "ScanlineVolumeShape";

		public ScanlineVolumeShape(FrostySdk.Ebx.ScanlineVolumeShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

