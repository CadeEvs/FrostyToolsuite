
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WalkerHealthComponentData))]
	public class WalkerHealthComponent : WSVehicleHealthComponent, IEntityData<FrostySdk.Ebx.WalkerHealthComponentData>
	{
		public new FrostySdk.Ebx.WalkerHealthComponentData Data => data as FrostySdk.Ebx.WalkerHealthComponentData;
		public override string DisplayName => "WalkerHealthComponent";

		public WalkerHealthComponent(FrostySdk.Ebx.WalkerHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

