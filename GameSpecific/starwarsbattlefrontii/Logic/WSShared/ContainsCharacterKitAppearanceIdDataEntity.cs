using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContainsCharacterKitAppearanceIdDataEntityData))]
	public class ContainsCharacterKitAppearanceIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.ContainsCharacterKitAppearanceIdDataEntityData>
	{
		public new FrostySdk.Ebx.ContainsCharacterKitAppearanceIdDataEntityData Data => data as FrostySdk.Ebx.ContainsCharacterKitAppearanceIdDataEntityData;
		public override string DisplayName => "ContainsCharacterKitAppearanceIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ContainsCharacterKitAppearanceIdDataEntity(FrostySdk.Ebx.ContainsCharacterKitAppearanceIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

