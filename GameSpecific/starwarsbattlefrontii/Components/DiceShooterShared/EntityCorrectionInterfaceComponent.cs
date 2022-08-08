
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EntityCorrectionInterfaceComponentData))]
	public class EntityCorrectionInterfaceComponent : GameComponent, IEntityData<FrostySdk.Ebx.EntityCorrectionInterfaceComponentData>
	{
		public new FrostySdk.Ebx.EntityCorrectionInterfaceComponentData Data => data as FrostySdk.Ebx.EntityCorrectionInterfaceComponentData;
		public override string DisplayName => "EntityCorrectionInterfaceComponent";

		public EntityCorrectionInterfaceComponent(FrostySdk.Ebx.EntityCorrectionInterfaceComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

