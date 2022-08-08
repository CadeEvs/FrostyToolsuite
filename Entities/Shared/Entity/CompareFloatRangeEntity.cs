using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareFloatRangeEntityData))]
#if GW2
	public class CompareFloatRangeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CompareFloatRangeEntityData>
#else
	public class CompareFloatRangeEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareFloatRangeEntityData>
#endif
	{
		public new FrostySdk.Ebx.CompareFloatRangeEntityData Data => data as FrostySdk.Ebx.CompareFloatRangeEntityData;
		public override string DisplayName => "CompareFloatRange";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareFloatRangeEntity(FrostySdk.Ebx.CompareFloatRangeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

