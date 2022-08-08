
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSVehicleHealthComponentData))]
	public class WSVehicleHealthComponent : ControllableHealthComponent, IEntityData<FrostySdk.Ebx.WSVehicleHealthComponentData>
	{
		public new FrostySdk.Ebx.WSVehicleHealthComponentData Data => data as FrostySdk.Ebx.WSVehicleHealthComponentData;
		public override string DisplayName => "WSVehicleHealthComponent";

		public WSVehicleHealthComponent(FrostySdk.Ebx.WSVehicleHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

