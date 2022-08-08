using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterKitAppearanceSkinGroupInfoEntityData))]
	public class CharacterKitAppearanceSkinGroupInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterKitAppearanceSkinGroupInfoEntityData>
	{
		public new FrostySdk.Ebx.CharacterKitAppearanceSkinGroupInfoEntityData Data => data as FrostySdk.Ebx.CharacterKitAppearanceSkinGroupInfoEntityData;
		public override string DisplayName => "CharacterKitAppearanceSkinGroupInfo";

		public CharacterKitAppearanceSkinGroupInfoEntity(FrostySdk.Ebx.CharacterKitAppearanceSkinGroupInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

