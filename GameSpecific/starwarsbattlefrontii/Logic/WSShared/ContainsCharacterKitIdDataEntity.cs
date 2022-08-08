using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContainsCharacterKitIdDataEntityData))]
	public class ContainsCharacterKitIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.ContainsCharacterKitIdDataEntityData>
	{
		public new FrostySdk.Ebx.ContainsCharacterKitIdDataEntityData Data => data as FrostySdk.Ebx.ContainsCharacterKitIdDataEntityData;
		public override string DisplayName => "ContainsCharacterKitIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ContainsCharacterKitIdDataEntity(FrostySdk.Ebx.ContainsCharacterKitIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

