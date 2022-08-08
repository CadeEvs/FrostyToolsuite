using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OwnerToQueryResultEntityData))]
	public class OwnerToQueryResultEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OwnerToQueryResultEntityData>
	{
		public new FrostySdk.Ebx.OwnerToQueryResultEntityData Data => data as FrostySdk.Ebx.OwnerToQueryResultEntityData;
		public override string DisplayName => "OwnerToQueryResult";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public OwnerToQueryResultEntity(FrostySdk.Ebx.OwnerToQueryResultEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

