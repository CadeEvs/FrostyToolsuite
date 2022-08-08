using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerHealthInfoEntityData))]
	public class PlayerHealthInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerHealthInfoEntityData>
	{
		public new FrostySdk.Ebx.PlayerHealthInfoEntityData Data => data as FrostySdk.Ebx.PlayerHealthInfoEntityData;
		public override string DisplayName => "PlayerHealthInfo";

		public PlayerHealthInfoEntity(FrostySdk.Ebx.PlayerHealthInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

