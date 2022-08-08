using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareFactionIdDataEntityData))]
	public class CompareFactionIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareFactionIdDataEntityData>
	{
		public new FrostySdk.Ebx.CompareFactionIdDataEntityData Data => data as FrostySdk.Ebx.CompareFactionIdDataEntityData;
		public override string DisplayName => "CompareFactionIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareFactionIdDataEntity(FrostySdk.Ebx.CompareFactionIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

