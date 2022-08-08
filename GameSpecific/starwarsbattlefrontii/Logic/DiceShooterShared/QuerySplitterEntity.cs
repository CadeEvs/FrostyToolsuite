using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.QuerySplitterEntityData))]
	public class QuerySplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.QuerySplitterEntityData>
	{
		public new FrostySdk.Ebx.QuerySplitterEntityData Data => data as FrostySdk.Ebx.QuerySplitterEntityData;
		public override string DisplayName => "QuerySplitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public QuerySplitterEntity(FrostySdk.Ebx.QuerySplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

