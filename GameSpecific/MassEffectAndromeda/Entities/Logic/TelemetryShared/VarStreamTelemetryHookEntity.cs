using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VarStreamTelemetryHookEntityData))]
	public class VarStreamTelemetryHookEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VarStreamTelemetryHookEntityData>
	{
		public new FrostySdk.Ebx.VarStreamTelemetryHookEntityData Data => data as FrostySdk.Ebx.VarStreamTelemetryHookEntityData;
		public override string DisplayName => "VarStreamTelemetryHook";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public VarStreamTelemetryHookEntity(FrostySdk.Ebx.VarStreamTelemetryHookEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

