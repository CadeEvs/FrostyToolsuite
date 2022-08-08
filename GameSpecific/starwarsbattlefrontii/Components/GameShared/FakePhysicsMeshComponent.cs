
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FakePhysicsMeshComponentData))]
	public class FakePhysicsMeshComponent : MeshComponent, IEntityData<FrostySdk.Ebx.FakePhysicsMeshComponentData>
	{
		public new FrostySdk.Ebx.FakePhysicsMeshComponentData Data => data as FrostySdk.Ebx.FakePhysicsMeshComponentData;
		public override string DisplayName => "FakePhysicsMeshComponent";

		public FakePhysicsMeshComponent(FrostySdk.Ebx.FakePhysicsMeshComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

