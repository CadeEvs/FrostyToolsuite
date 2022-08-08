
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionHealComponentData))]
	public class DroidCompanionHealComponent : GameComponent, IEntityData<FrostySdk.Ebx.DroidCompanionHealComponentData>
	{
		public new FrostySdk.Ebx.DroidCompanionHealComponentData Data => data as FrostySdk.Ebx.DroidCompanionHealComponentData;
		public override string DisplayName => "DroidCompanionHealComponent";

		public DroidCompanionHealComponent(FrostySdk.Ebx.DroidCompanionHealComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

