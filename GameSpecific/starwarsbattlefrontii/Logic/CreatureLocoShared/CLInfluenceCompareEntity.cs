using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CLInfluenceCompareEntityData))]
	public class CLInfluenceCompareEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CLInfluenceCompareEntityData>
	{
		public new FrostySdk.Ebx.CLInfluenceCompareEntityData Data => data as FrostySdk.Ebx.CLInfluenceCompareEntityData;
		public override string DisplayName => "CLInfluenceCompare";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CLInfluenceCompareEntity(FrostySdk.Ebx.CLInfluenceCompareEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

