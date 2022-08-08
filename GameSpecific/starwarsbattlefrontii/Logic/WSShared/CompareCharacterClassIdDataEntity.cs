using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareCharacterClassIdDataEntityData))]
	public class CompareCharacterClassIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareCharacterClassIdDataEntityData>
	{
		public new FrostySdk.Ebx.CompareCharacterClassIdDataEntityData Data => data as FrostySdk.Ebx.CompareCharacterClassIdDataEntityData;
		public override string DisplayName => "CompareCharacterClassIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareCharacterClassIdDataEntity(FrostySdk.Ebx.CompareCharacterClassIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

