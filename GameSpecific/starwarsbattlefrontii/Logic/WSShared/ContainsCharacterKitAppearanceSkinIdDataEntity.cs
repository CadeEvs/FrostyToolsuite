using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContainsCharacterKitAppearanceSkinIdDataEntityData))]
	public class ContainsCharacterKitAppearanceSkinIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.ContainsCharacterKitAppearanceSkinIdDataEntityData>
	{
		public new FrostySdk.Ebx.ContainsCharacterKitAppearanceSkinIdDataEntityData Data => data as FrostySdk.Ebx.ContainsCharacterKitAppearanceSkinIdDataEntityData;
		public override string DisplayName => "ContainsCharacterKitAppearanceSkinIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ContainsCharacterKitAppearanceSkinIdDataEntity(FrostySdk.Ebx.ContainsCharacterKitAppearanceSkinIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

