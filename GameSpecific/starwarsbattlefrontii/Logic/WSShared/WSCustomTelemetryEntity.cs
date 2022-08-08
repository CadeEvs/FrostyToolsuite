using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSCustomTelemetryEntityData))]
	public class WSCustomTelemetryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSCustomTelemetryEntityData>
	{
		public new FrostySdk.Ebx.WSCustomTelemetryEntityData Data => data as FrostySdk.Ebx.WSCustomTelemetryEntityData;
		public override string DisplayName => "WSCustomTelemetry";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSCustomTelemetryEntity(FrostySdk.Ebx.WSCustomTelemetryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

