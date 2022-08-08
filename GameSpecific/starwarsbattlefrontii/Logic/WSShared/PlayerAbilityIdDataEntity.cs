using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerAbilityIdDataEntityData))]
	public class PlayerAbilityIdDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerAbilityIdDataEntityData>
	{
		public new FrostySdk.Ebx.PlayerAbilityIdDataEntityData Data => data as FrostySdk.Ebx.PlayerAbilityIdDataEntityData;
		public override string DisplayName => "PlayerAbilityIdData";

		public PlayerAbilityIdDataEntity(FrostySdk.Ebx.PlayerAbilityIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

