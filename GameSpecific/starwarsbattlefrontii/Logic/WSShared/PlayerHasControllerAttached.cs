using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerHasControllerAttachedData))]
	public class PlayerHasControllerAttached : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerHasControllerAttachedData>
	{
		public new FrostySdk.Ebx.PlayerHasControllerAttachedData Data => data as FrostySdk.Ebx.PlayerHasControllerAttachedData;
		public override string DisplayName => "PlayerHasControllerAttached";

		public PlayerHasControllerAttached(FrostySdk.Ebx.PlayerHasControllerAttachedData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

