using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FriendZoneEntityData))]
	public class FriendZoneEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FriendZoneEntityData>
	{
		public new FrostySdk.Ebx.FriendZoneEntityData Data => data as FrostySdk.Ebx.FriendZoneEntityData;
		public override string DisplayName => "FriendZone";

		public FriendZoneEntity(FrostySdk.Ebx.FriendZoneEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

