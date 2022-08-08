
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionThreatAlertComponentData))]
	public class DroidCompanionThreatAlertComponent : GameComponent, IEntityData<FrostySdk.Ebx.DroidCompanionThreatAlertComponentData>
	{
		public new FrostySdk.Ebx.DroidCompanionThreatAlertComponentData Data => data as FrostySdk.Ebx.DroidCompanionThreatAlertComponentData;
		public override string DisplayName => "DroidCompanionThreatAlertComponent";

		public DroidCompanionThreatAlertComponent(FrostySdk.Ebx.DroidCompanionThreatAlertComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

