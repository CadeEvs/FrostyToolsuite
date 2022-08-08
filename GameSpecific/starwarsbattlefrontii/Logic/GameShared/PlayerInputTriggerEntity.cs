using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerInputTriggerEntityData))]
	public class PlayerInputTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerInputTriggerEntityData>
	{
		public new FrostySdk.Ebx.PlayerInputTriggerEntityData Data => data as FrostySdk.Ebx.PlayerInputTriggerEntityData;
		public override string DisplayName => "PlayerInputTrigger";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayerInputTriggerEntity(FrostySdk.Ebx.PlayerInputTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

