using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.QueryResultPlayerInspectorEntityData))]
	public class QueryResultPlayerInspectorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.QueryResultPlayerInspectorEntityData>
	{
		public new FrostySdk.Ebx.QueryResultPlayerInspectorEntityData Data => data as FrostySdk.Ebx.QueryResultPlayerInspectorEntityData;
		public override string DisplayName => "QueryResultPlayerInspector";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public QueryResultPlayerInspectorEntity(FrostySdk.Ebx.QueryResultPlayerInspectorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

