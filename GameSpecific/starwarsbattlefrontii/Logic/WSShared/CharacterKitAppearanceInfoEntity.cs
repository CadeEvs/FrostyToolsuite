using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterKitAppearanceInfoEntityData))]
	public class CharacterKitAppearanceInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterKitAppearanceInfoEntityData>
	{
		public new FrostySdk.Ebx.CharacterKitAppearanceInfoEntityData Data => data as FrostySdk.Ebx.CharacterKitAppearanceInfoEntityData;
		public override string DisplayName => "CharacterKitAppearanceInfo";

		public CharacterKitAppearanceInfoEntity(FrostySdk.Ebx.CharacterKitAppearanceInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

