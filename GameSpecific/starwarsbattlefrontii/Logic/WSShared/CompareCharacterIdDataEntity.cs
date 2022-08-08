using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareCharacterIdDataEntityData))]
	public class CompareCharacterIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareCharacterIdDataEntityData>
	{
		public new FrostySdk.Ebx.CompareCharacterIdDataEntityData Data => data as FrostySdk.Ebx.CompareCharacterIdDataEntityData;
		public override string DisplayName => "CompareCharacterIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareCharacterIdDataEntity(FrostySdk.Ebx.CompareCharacterIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

