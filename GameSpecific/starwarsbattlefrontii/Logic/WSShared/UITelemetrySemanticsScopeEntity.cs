using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UITelemetrySemanticsScopeEntityData))]
	public class UITelemetrySemanticsScopeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UITelemetrySemanticsScopeEntityData>
	{
		public new FrostySdk.Ebx.UITelemetrySemanticsScopeEntityData Data => data as FrostySdk.Ebx.UITelemetrySemanticsScopeEntityData;
		public override string DisplayName => "UITelemetrySemanticsScope";

		public UITelemetrySemanticsScopeEntity(FrostySdk.Ebx.UITelemetrySemanticsScopeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

