using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Int64HubEntityData))]
	public class Int64HubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.Int64HubEntityData>
	{
		public new FrostySdk.Ebx.Int64HubEntityData Data => data as FrostySdk.Ebx.Int64HubEntityData;
		public override string DisplayName => "Int64Hub";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public Int64HubEntity(FrostySdk.Ebx.Int64HubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

