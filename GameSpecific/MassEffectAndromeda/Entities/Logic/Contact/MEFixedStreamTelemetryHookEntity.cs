using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEFixedStreamTelemetryHookEntityData))]
	public class MEFixedStreamTelemetryHookEntity : FixedStreamTelemetryHookEntity, IEntityData<FrostySdk.Ebx.MEFixedStreamTelemetryHookEntityData>
	{
		public new FrostySdk.Ebx.MEFixedStreamTelemetryHookEntityData Data => data as FrostySdk.Ebx.MEFixedStreamTelemetryHookEntityData;
		public override string DisplayName => "MEFixedStreamTelemetryHook";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MEFixedStreamTelemetryHookEntity(FrostySdk.Ebx.MEFixedStreamTelemetryHookEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

