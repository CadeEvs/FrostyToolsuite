using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterKitAppearanceIdDataEntityData))]
	public class CharacterKitAppearanceIdDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterKitAppearanceIdDataEntityData>
	{
		public new FrostySdk.Ebx.CharacterKitAppearanceIdDataEntityData Data => data as FrostySdk.Ebx.CharacterKitAppearanceIdDataEntityData;
		public override string DisplayName => "CharacterKitAppearanceIdData";

		public CharacterKitAppearanceIdDataEntity(FrostySdk.Ebx.CharacterKitAppearanceIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

