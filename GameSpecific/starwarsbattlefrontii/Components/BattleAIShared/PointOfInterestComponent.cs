
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PointOfInterestComponentData))]
	public class PointOfInterestComponent : GameComponent, IEntityData<FrostySdk.Ebx.PointOfInterestComponentData>
	{
		public new FrostySdk.Ebx.PointOfInterestComponentData Data => data as FrostySdk.Ebx.PointOfInterestComponentData;
		public override string DisplayName => "PointOfInterestComponent";

		public PointOfInterestComponent(FrostySdk.Ebx.PointOfInterestComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

