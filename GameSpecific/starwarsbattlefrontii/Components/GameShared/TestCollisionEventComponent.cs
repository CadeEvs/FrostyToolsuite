
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestCollisionEventComponentData))]
	public class TestCollisionEventComponent : Component, IEntityData<FrostySdk.Ebx.TestCollisionEventComponentData>
	{
		public new FrostySdk.Ebx.TestCollisionEventComponentData Data => data as FrostySdk.Ebx.TestCollisionEventComponentData;
		public override string DisplayName => "TestCollisionEventComponent";

		public TestCollisionEventComponent(FrostySdk.Ebx.TestCollisionEventComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

