
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RemoteEntryComponentData))]
	public class RemoteEntryComponent : DiceShooterVehicleEntryComponent, IEntityData<FrostySdk.Ebx.RemoteEntryComponentData>
	{
		public new FrostySdk.Ebx.RemoteEntryComponentData Data => data as FrostySdk.Ebx.RemoteEntryComponentData;
		public override string DisplayName => "RemoteEntryComponent";

		public RemoteEntryComponent(FrostySdk.Ebx.RemoteEntryComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

