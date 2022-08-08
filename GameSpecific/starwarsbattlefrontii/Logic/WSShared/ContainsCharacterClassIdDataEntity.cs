using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContainsCharacterClassIdDataEntityData))]
	public class ContainsCharacterClassIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.ContainsCharacterClassIdDataEntityData>
	{
		public new FrostySdk.Ebx.ContainsCharacterClassIdDataEntityData Data => data as FrostySdk.Ebx.ContainsCharacterClassIdDataEntityData;
		public override string DisplayName => "ContainsCharacterClassIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ContainsCharacterClassIdDataEntity(FrostySdk.Ebx.ContainsCharacterClassIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

