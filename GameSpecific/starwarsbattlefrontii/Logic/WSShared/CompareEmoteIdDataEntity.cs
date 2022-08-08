using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareEmoteIdDataEntityData))]
	public class CompareEmoteIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareEmoteIdDataEntityData>
	{
		public new FrostySdk.Ebx.CompareEmoteIdDataEntityData Data => data as FrostySdk.Ebx.CompareEmoteIdDataEntityData;
		public override string DisplayName => "CompareEmoteIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareEmoteIdDataEntity(FrostySdk.Ebx.CompareEmoteIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

