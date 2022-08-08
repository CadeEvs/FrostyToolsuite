using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RoundTelemetryEntityData))]
	public class RoundTelemetryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RoundTelemetryEntityData>
	{
		public new FrostySdk.Ebx.RoundTelemetryEntityData Data => data as FrostySdk.Ebx.RoundTelemetryEntityData;
		public override string DisplayName => "RoundTelemetry";

		public RoundTelemetryEntity(FrostySdk.Ebx.RoundTelemetryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

