
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoneCollisionComponentData))]
	public class BoneCollisionComponent : GameComponent, IEntityData<FrostySdk.Ebx.BoneCollisionComponentData>
	{
		public new FrostySdk.Ebx.BoneCollisionComponentData Data => data as FrostySdk.Ebx.BoneCollisionComponentData;
		public override string DisplayName => "BoneCollisionComponent";

		public BoneCollisionComponent(FrostySdk.Ebx.BoneCollisionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

