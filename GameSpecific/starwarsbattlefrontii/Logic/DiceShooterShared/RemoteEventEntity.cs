using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RemoteEventEntityData))]
	public class RemoteEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RemoteEventEntityData>
	{
		public new FrostySdk.Ebx.RemoteEventEntityData Data => data as FrostySdk.Ebx.RemoteEventEntityData;
		public override string DisplayName => "RemoteEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RemoteEventEntity(FrostySdk.Ebx.RemoteEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

