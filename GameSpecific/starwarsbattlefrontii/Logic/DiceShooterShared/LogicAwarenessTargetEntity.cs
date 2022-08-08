using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LogicAwarenessTargetEntityData))]
	public class LogicAwarenessTargetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LogicAwarenessTargetEntityData>
	{
		public new FrostySdk.Ebx.LogicAwarenessTargetEntityData Data => data as FrostySdk.Ebx.LogicAwarenessTargetEntityData;
		public override string DisplayName => "LogicAwarenessTarget";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LogicAwarenessTargetEntity(FrostySdk.Ebx.LogicAwarenessTargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

