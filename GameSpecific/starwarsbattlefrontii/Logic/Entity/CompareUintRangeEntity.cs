using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareUintRangeEntityData))]
	public class CompareUintRangeEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareUintRangeEntityData>
	{
		public new FrostySdk.Ebx.CompareUintRangeEntityData Data => data as FrostySdk.Ebx.CompareUintRangeEntityData;
		public override string DisplayName => "CompareUintRange";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareUintRangeEntity(FrostySdk.Ebx.CompareUintRangeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

