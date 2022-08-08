using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RemoteStateEntityData))]
	public class RemoteStateEntity : SimpleStateEntityBase, IEntityData<FrostySdk.Ebx.RemoteStateEntityData>
	{
		public new FrostySdk.Ebx.RemoteStateEntityData Data => data as FrostySdk.Ebx.RemoteStateEntityData;
		public override string DisplayName => "RemoteState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RemoteStateEntity(FrostySdk.Ebx.RemoteStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

