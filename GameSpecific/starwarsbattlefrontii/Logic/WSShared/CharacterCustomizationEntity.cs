using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterCustomizationEntityData))]
	public class CharacterCustomizationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterCustomizationEntityData>
	{
		public new FrostySdk.Ebx.CharacterCustomizationEntityData Data => data as FrostySdk.Ebx.CharacterCustomizationEntityData;
		public override string DisplayName => "CharacterCustomization";

		public CharacterCustomizationEntity(FrostySdk.Ebx.CharacterCustomizationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

