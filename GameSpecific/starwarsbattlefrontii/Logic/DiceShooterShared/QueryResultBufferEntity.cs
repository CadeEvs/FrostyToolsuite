using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.QueryResultBufferEntityData))]
	public class QueryResultBufferEntity : LogicEntity, IEntityData<FrostySdk.Ebx.QueryResultBufferEntityData>
	{
		public new FrostySdk.Ebx.QueryResultBufferEntityData Data => data as FrostySdk.Ebx.QueryResultBufferEntityData;
		public override string DisplayName => "QueryResultBuffer";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public QueryResultBufferEntity(FrostySdk.Ebx.QueryResultBufferEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

