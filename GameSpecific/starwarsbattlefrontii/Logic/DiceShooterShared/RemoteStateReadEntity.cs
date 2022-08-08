using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RemoteStateReadEntityData))]
	public class RemoteStateReadEntity : SimpleStateEntityBase, IEntityData<FrostySdk.Ebx.RemoteStateReadEntityData>
	{
		public new FrostySdk.Ebx.RemoteStateReadEntityData Data => data as FrostySdk.Ebx.RemoteStateReadEntityData;
		public override string DisplayName => "RemoteStateRead";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RemoteStateReadEntity(FrostySdk.Ebx.RemoteStateReadEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

