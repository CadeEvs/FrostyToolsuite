using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterUnspentSkillPointEntityData))]
	public class CharacterUnspentSkillPointEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterUnspentSkillPointEntityData>
	{
		public new FrostySdk.Ebx.CharacterUnspentSkillPointEntityData Data => data as FrostySdk.Ebx.CharacterUnspentSkillPointEntityData;
		public override string DisplayName => "CharacterUnspentSkillPoint";

		public CharacterUnspentSkillPointEntity(FrostySdk.Ebx.CharacterUnspentSkillPointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

