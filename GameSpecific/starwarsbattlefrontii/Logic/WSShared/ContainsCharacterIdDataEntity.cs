using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContainsCharacterIdDataEntityData))]
	public class ContainsCharacterIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.ContainsCharacterIdDataEntityData>
	{
		public new FrostySdk.Ebx.ContainsCharacterIdDataEntityData Data => data as FrostySdk.Ebx.ContainsCharacterIdDataEntityData;
		public override string DisplayName => "ContainsCharacterIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ContainsCharacterIdDataEntity(FrostySdk.Ebx.ContainsCharacterIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

