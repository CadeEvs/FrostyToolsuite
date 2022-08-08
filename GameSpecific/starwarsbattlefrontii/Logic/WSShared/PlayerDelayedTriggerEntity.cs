using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerDelayedTriggerEntityData))]
	public class PlayerDelayedTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerDelayedTriggerEntityData>
	{
		public new FrostySdk.Ebx.PlayerDelayedTriggerEntityData Data => data as FrostySdk.Ebx.PlayerDelayedTriggerEntityData;
		public override string DisplayName => "PlayerDelayedTrigger";

		public PlayerDelayedTriggerEntity(FrostySdk.Ebx.PlayerDelayedTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

