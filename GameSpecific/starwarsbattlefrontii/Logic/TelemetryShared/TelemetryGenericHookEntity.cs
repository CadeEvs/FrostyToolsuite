using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TelemetryGenericHookEntityData))]
	public class TelemetryGenericHookEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TelemetryGenericHookEntityData>
	{
		public new FrostySdk.Ebx.TelemetryGenericHookEntityData Data => data as FrostySdk.Ebx.TelemetryGenericHookEntityData;
		public override string DisplayName => "TelemetryGenericHook";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TelemetryGenericHookEntity(FrostySdk.Ebx.TelemetryGenericHookEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

