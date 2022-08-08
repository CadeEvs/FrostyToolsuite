using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RemoteStateQueryFilterEntityData))]
	public class RemoteStateQueryFilterEntity : SimpleStateEntityBase, IEntityData<FrostySdk.Ebx.RemoteStateQueryFilterEntityData>
	{
		public new FrostySdk.Ebx.RemoteStateQueryFilterEntityData Data => data as FrostySdk.Ebx.RemoteStateQueryFilterEntityData;
		public override string DisplayName => "RemoteStateQueryFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RemoteStateQueryFilterEntity(FrostySdk.Ebx.RemoteStateQueryFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

