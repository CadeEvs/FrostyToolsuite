using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterKitAppearanceSkinIdDataEntityData))]
	public class CharacterKitAppearanceSkinIdDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterKitAppearanceSkinIdDataEntityData>
	{
		public new FrostySdk.Ebx.CharacterKitAppearanceSkinIdDataEntityData Data => data as FrostySdk.Ebx.CharacterKitAppearanceSkinIdDataEntityData;
		public override string DisplayName => "CharacterKitAppearanceSkinIdData";

		public CharacterKitAppearanceSkinIdDataEntity(FrostySdk.Ebx.CharacterKitAppearanceSkinIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

