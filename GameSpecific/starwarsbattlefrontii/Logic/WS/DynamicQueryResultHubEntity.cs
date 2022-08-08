using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicQueryResultHubEntityData))]
	public class DynamicQueryResultHubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DynamicQueryResultHubEntityData>
	{
		public new FrostySdk.Ebx.DynamicQueryResultHubEntityData Data => data as FrostySdk.Ebx.DynamicQueryResultHubEntityData;
		public override string DisplayName => "DynamicQueryResultHub";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DynamicQueryResultHubEntity(FrostySdk.Ebx.DynamicQueryResultHubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

