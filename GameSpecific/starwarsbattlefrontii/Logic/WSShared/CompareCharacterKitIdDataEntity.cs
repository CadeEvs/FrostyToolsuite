using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareCharacterKitIdDataEntityData))]
	public class CompareCharacterKitIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareCharacterKitIdDataEntityData>
	{
		public new FrostySdk.Ebx.CompareCharacterKitIdDataEntityData Data => data as FrostySdk.Ebx.CompareCharacterKitIdDataEntityData;
		public override string DisplayName => "CompareCharacterKitIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareCharacterKitIdDataEntity(FrostySdk.Ebx.CompareCharacterKitIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

