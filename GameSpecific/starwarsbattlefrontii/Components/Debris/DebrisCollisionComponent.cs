
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebrisCollisionComponentData))]
	public class DebrisCollisionComponent : Component, IEntityData<FrostySdk.Ebx.DebrisCollisionComponentData>
	{
		public new FrostySdk.Ebx.DebrisCollisionComponentData Data => data as FrostySdk.Ebx.DebrisCollisionComponentData;
		public override string DisplayName => "DebrisCollisionComponent";

		public DebrisCollisionComponent(FrostySdk.Ebx.DebrisCollisionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

