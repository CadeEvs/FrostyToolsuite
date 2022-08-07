using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectInt64EntityData))]
	public class SelectInt64Entity : SelectPropertyEntity, IEntityData<FrostySdk.Ebx.SelectInt64EntityData>
	{
		public new FrostySdk.Ebx.SelectInt64EntityData Data => data as FrostySdk.Ebx.SelectInt64EntityData;
		public override string DisplayName => "SelectInt64";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SelectInt64Entity(FrostySdk.Ebx.SelectInt64EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

