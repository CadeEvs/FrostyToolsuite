using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterCostInfoEntityData))]
	public class CharacterCostInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterCostInfoEntityData>
	{
		public new FrostySdk.Ebx.CharacterCostInfoEntityData Data => data as FrostySdk.Ebx.CharacterCostInfoEntityData;
		public override string DisplayName => "CharacterCostInfo";

		public CharacterCostInfoEntity(FrostySdk.Ebx.CharacterCostInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

