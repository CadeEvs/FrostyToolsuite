
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleExitPointComponentData))]
	public class VehicleExitPointComponent : GameComponent, IEntityData<FrostySdk.Ebx.VehicleExitPointComponentData>
	{
		public new FrostySdk.Ebx.VehicleExitPointComponentData Data => data as FrostySdk.Ebx.VehicleExitPointComponentData;
		public override string DisplayName => "VehicleExitPointComponent";

		public VehicleExitPointComponent(FrostySdk.Ebx.VehicleExitPointComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

