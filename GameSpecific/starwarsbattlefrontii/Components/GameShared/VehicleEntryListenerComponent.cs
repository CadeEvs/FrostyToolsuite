
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleEntryListenerComponentData))]
	public class VehicleEntryListenerComponent : GameComponent, IEntityData<FrostySdk.Ebx.VehicleEntryListenerComponentData>
	{
		public new FrostySdk.Ebx.VehicleEntryListenerComponentData Data => data as FrostySdk.Ebx.VehicleEntryListenerComponentData;
		public override string DisplayName => "VehicleEntryListenerComponent";

		public VehicleEntryListenerComponent(FrostySdk.Ebx.VehicleEntryListenerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

