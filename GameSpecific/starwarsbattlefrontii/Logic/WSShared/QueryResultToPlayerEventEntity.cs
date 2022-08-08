using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.QueryResultToPlayerEventEntityData))]
	public class QueryResultToPlayerEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.QueryResultToPlayerEventEntityData>
	{
		public new FrostySdk.Ebx.QueryResultToPlayerEventEntityData Data => data as FrostySdk.Ebx.QueryResultToPlayerEventEntityData;
		public override string DisplayName => "QueryResultToPlayerEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public QueryResultToPlayerEventEntity(FrostySdk.Ebx.QueryResultToPlayerEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

