using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransactionalTelemetryHookEntityData))]
	public class TransactionalTelemetryHookEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransactionalTelemetryHookEntityData>
	{
		public new FrostySdk.Ebx.TransactionalTelemetryHookEntityData Data => data as FrostySdk.Ebx.TransactionalTelemetryHookEntityData;
		public override string DisplayName => "TransactionalTelemetryHook";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TransactionalTelemetryHookEntity(FrostySdk.Ebx.TransactionalTelemetryHookEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

