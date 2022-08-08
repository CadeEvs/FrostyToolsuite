
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterAnimatableMeshProxyComponentData))]
	public class CharacterAnimatableMeshProxyComponent : MeshComponent, IEntityData<FrostySdk.Ebx.CharacterAnimatableMeshProxyComponentData>
	{
		public new FrostySdk.Ebx.CharacterAnimatableMeshProxyComponentData Data => data as FrostySdk.Ebx.CharacterAnimatableMeshProxyComponentData;
		public override string DisplayName => "CharacterAnimatableMeshProxyComponent";

		public CharacterAnimatableMeshProxyComponent(FrostySdk.Ebx.CharacterAnimatableMeshProxyComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

