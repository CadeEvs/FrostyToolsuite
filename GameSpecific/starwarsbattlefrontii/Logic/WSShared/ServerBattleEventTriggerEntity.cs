using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ServerBattleEventTriggerEntityData))]
	public class ServerBattleEventTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ServerBattleEventTriggerEntityData>
	{
		public new FrostySdk.Ebx.ServerBattleEventTriggerEntityData Data => data as FrostySdk.Ebx.ServerBattleEventTriggerEntityData;
		public override string DisplayName => "ServerBattleEventTrigger";

		public ServerBattleEventTriggerEntity(FrostySdk.Ebx.ServerBattleEventTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

