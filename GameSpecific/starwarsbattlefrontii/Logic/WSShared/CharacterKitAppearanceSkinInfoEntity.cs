using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterKitAppearanceSkinInfoEntityData))]
	public class CharacterKitAppearanceSkinInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterKitAppearanceSkinInfoEntityData>
	{
		public new FrostySdk.Ebx.CharacterKitAppearanceSkinInfoEntityData Data => data as FrostySdk.Ebx.CharacterKitAppearanceSkinInfoEntityData;
		public override string DisplayName => "CharacterKitAppearanceSkinInfo";

		public CharacterKitAppearanceSkinInfoEntity(FrostySdk.Ebx.CharacterKitAppearanceSkinInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

