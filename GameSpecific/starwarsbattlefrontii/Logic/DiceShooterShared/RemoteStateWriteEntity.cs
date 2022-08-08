using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RemoteStateWriteEntityData))]
	public class RemoteStateWriteEntity : SimpleStateEntityBase, IEntityData<FrostySdk.Ebx.RemoteStateWriteEntityData>
	{
		public new FrostySdk.Ebx.RemoteStateWriteEntityData Data => data as FrostySdk.Ebx.RemoteStateWriteEntityData;
		public override string DisplayName => "RemoteStateWrite";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RemoteStateWriteEntity(FrostySdk.Ebx.RemoteStateWriteEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

