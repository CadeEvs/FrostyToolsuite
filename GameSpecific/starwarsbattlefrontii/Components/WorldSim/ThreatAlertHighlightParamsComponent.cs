
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ThreatAlertHighlightParamsComponentData))]
	public class ThreatAlertHighlightParamsComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.ThreatAlertHighlightParamsComponentData>
	{
		public new FrostySdk.Ebx.ThreatAlertHighlightParamsComponentData Data => data as FrostySdk.Ebx.ThreatAlertHighlightParamsComponentData;
		public override string DisplayName => "ThreatAlertHighlightParamsComponent";

		public ThreatAlertHighlightParamsComponent(FrostySdk.Ebx.ThreatAlertHighlightParamsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

