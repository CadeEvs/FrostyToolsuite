using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerLookAtEntityData))]
	public class PlayerLookAtEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerLookAtEntityData>
	{
		public new FrostySdk.Ebx.PlayerLookAtEntityData Data => data as FrostySdk.Ebx.PlayerLookAtEntityData;
		public override string DisplayName => "PlayerLookAt";

		public PlayerLookAtEntity(FrostySdk.Ebx.PlayerLookAtEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

