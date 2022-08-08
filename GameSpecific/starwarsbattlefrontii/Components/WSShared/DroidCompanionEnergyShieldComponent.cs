
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionEnergyShieldComponentData))]
	public class DroidCompanionEnergyShieldComponent : GameComponent, IEntityData<FrostySdk.Ebx.DroidCompanionEnergyShieldComponentData>
	{
		public new FrostySdk.Ebx.DroidCompanionEnergyShieldComponentData Data => data as FrostySdk.Ebx.DroidCompanionEnergyShieldComponentData;
		public override string DisplayName => "DroidCompanionEnergyShieldComponent";

		public DroidCompanionEnergyShieldComponent(FrostySdk.Ebx.DroidCompanionEnergyShieldComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

