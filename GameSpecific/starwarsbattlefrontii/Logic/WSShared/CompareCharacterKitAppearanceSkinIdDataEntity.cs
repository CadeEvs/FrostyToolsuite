using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareCharacterKitAppearanceSkinIdDataEntityData))]
	public class CompareCharacterKitAppearanceSkinIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareCharacterKitAppearanceSkinIdDataEntityData>
	{
		public new FrostySdk.Ebx.CompareCharacterKitAppearanceSkinIdDataEntityData Data => data as FrostySdk.Ebx.CompareCharacterKitAppearanceSkinIdDataEntityData;
		public override string DisplayName => "CompareCharacterKitAppearanceSkinIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareCharacterKitAppearanceSkinIdDataEntity(FrostySdk.Ebx.CompareCharacterKitAppearanceSkinIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

