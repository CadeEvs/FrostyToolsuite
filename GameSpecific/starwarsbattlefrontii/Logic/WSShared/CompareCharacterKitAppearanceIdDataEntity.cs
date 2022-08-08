using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareCharacterKitAppearanceIdDataEntityData))]
	public class CompareCharacterKitAppearanceIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareCharacterKitAppearanceIdDataEntityData>
	{
		public new FrostySdk.Ebx.CompareCharacterKitAppearanceIdDataEntityData Data => data as FrostySdk.Ebx.CompareCharacterKitAppearanceIdDataEntityData;
		public override string DisplayName => "CompareCharacterKitAppearanceIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareCharacterKitAppearanceIdDataEntity(FrostySdk.Ebx.CompareCharacterKitAppearanceIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

