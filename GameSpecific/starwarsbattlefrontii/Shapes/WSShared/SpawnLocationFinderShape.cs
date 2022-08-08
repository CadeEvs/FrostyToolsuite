using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnLocationFinderShapeData))]
	public class SpawnLocationFinderShape : VolumeVectorShape, IEntityData<FrostySdk.Ebx.SpawnLocationFinderShapeData>
	{
		public new FrostySdk.Ebx.SpawnLocationFinderShapeData Data => data as FrostySdk.Ebx.SpawnLocationFinderShapeData;
		public override string DisplayName => "SpawnLocationFinderShape";

		public SpawnLocationFinderShape(FrostySdk.Ebx.SpawnLocationFinderShapeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

