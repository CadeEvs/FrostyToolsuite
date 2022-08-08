using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.QueryCacheEntityData))]
	public class QueryCacheEntity : LogicEntity, IEntityData<FrostySdk.Ebx.QueryCacheEntityData>
	{
		public new FrostySdk.Ebx.QueryCacheEntityData Data => data as FrostySdk.Ebx.QueryCacheEntityData;
		public override string DisplayName => "QueryCache";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public QueryCacheEntity(FrostySdk.Ebx.QueryCacheEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

