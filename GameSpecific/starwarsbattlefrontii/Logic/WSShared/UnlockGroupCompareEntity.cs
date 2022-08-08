using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UnlockGroupCompareEntityData))]
	public class UnlockGroupCompareEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UnlockGroupCompareEntityData>
	{
		public new FrostySdk.Ebx.UnlockGroupCompareEntityData Data => data as FrostySdk.Ebx.UnlockGroupCompareEntityData;
		public override string DisplayName => "UnlockGroupCompare";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UnlockGroupCompareEntity(FrostySdk.Ebx.UnlockGroupCompareEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

