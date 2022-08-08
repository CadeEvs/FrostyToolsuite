using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.QueryFilterEntityData))]
	public class QueryFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.QueryFilterEntityData>
	{
		public new FrostySdk.Ebx.QueryFilterEntityData Data => data as FrostySdk.Ebx.QueryFilterEntityData;
		public override string DisplayName => "QueryFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public QueryFilterEntity(FrostySdk.Ebx.QueryFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

