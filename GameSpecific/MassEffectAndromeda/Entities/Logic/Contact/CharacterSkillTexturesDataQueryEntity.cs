using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterSkillTexturesDataQueryEntityData))]
	public class CharacterSkillTexturesDataQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterSkillTexturesDataQueryEntityData>
	{
		public new FrostySdk.Ebx.CharacterSkillTexturesDataQueryEntityData Data => data as FrostySdk.Ebx.CharacterSkillTexturesDataQueryEntityData;
		public override string DisplayName => "CharacterSkillTexturesDataQuery";

		public CharacterSkillTexturesDataQueryEntity(FrostySdk.Ebx.CharacterSkillTexturesDataQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

