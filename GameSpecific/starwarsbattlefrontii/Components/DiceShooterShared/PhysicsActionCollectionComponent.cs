
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsActionCollectionComponentData))]
	public class PhysicsActionCollectionComponent : GameComponent, IEntityData<FrostySdk.Ebx.PhysicsActionCollectionComponentData>
	{
		public new FrostySdk.Ebx.PhysicsActionCollectionComponentData Data => data as FrostySdk.Ebx.PhysicsActionCollectionComponentData;
		public override string DisplayName => "PhysicsActionCollectionComponent";

		public PhysicsActionCollectionComponent(FrostySdk.Ebx.PhysicsActionCollectionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

