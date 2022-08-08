using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSTelemetryHookEntityData))]
	public class WSTelemetryHookEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSTelemetryHookEntityData>
	{
		public new FrostySdk.Ebx.WSTelemetryHookEntityData Data => data as FrostySdk.Ebx.WSTelemetryHookEntityData;
		public override string DisplayName => "WSTelemetryHook";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSTelemetryHookEntity(FrostySdk.Ebx.WSTelemetryHookEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

