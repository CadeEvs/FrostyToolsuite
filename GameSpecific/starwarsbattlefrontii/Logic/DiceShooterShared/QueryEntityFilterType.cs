using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.QueryEntityFilterTypeData))]
	public class QueryEntityFilterType : LogicEntity, IEntityData<FrostySdk.Ebx.QueryEntityFilterTypeData>
	{
		public new FrostySdk.Ebx.QueryEntityFilterTypeData Data => data as FrostySdk.Ebx.QueryEntityFilterTypeData;
		public override string DisplayName => "QueryEntityFilterType";

		public QueryEntityFilterType(FrostySdk.Ebx.QueryEntityFilterTypeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

