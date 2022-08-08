
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EntityReplicationInterfaceComponentData))]
	public class EntityReplicationInterfaceComponent : GameComponent, IEntityData<FrostySdk.Ebx.EntityReplicationInterfaceComponentData>
	{
		public new FrostySdk.Ebx.EntityReplicationInterfaceComponentData Data => data as FrostySdk.Ebx.EntityReplicationInterfaceComponentData;
		public override string DisplayName => "EntityReplicationInterfaceComponent";

		public EntityReplicationInterfaceComponent(FrostySdk.Ebx.EntityReplicationInterfaceComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

