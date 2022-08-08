
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MeshComponentData))]
	public class MeshComponent : GameComponent, IEntityData<FrostySdk.Ebx.MeshComponentData>
	{
		public new FrostySdk.Ebx.MeshComponentData Data => data as FrostySdk.Ebx.MeshComponentData;
		public override string DisplayName => "MeshComponent";

		public MeshComponent(FrostySdk.Ebx.MeshComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

