using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TelemetryHookEntityData))]
	public class TelemetryHookEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TelemetryHookEntityData>
	{
		public new FrostySdk.Ebx.TelemetryHookEntityData Data => data as FrostySdk.Ebx.TelemetryHookEntityData;
		public override string DisplayName => "TelemetryHook";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TelemetryHookEntity(FrostySdk.Ebx.TelemetryHookEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

