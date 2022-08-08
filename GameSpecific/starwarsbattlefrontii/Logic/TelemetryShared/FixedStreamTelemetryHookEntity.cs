using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FixedStreamTelemetryHookEntityData))]
	public class FixedStreamTelemetryHookEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FixedStreamTelemetryHookEntityData>
	{
		public new FrostySdk.Ebx.FixedStreamTelemetryHookEntityData Data => data as FrostySdk.Ebx.FixedStreamTelemetryHookEntityData;
		public override string DisplayName => "FixedStreamTelemetryHook";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FixedStreamTelemetryHookEntity(FrostySdk.Ebx.FixedStreamTelemetryHookEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

