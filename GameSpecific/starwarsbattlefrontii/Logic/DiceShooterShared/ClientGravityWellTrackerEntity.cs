using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientGravityWellTrackerEntityData))]
	public class ClientGravityWellTrackerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientGravityWellTrackerEntityData>
	{
		public new FrostySdk.Ebx.ClientGravityWellTrackerEntityData Data => data as FrostySdk.Ebx.ClientGravityWellTrackerEntityData;
		public override string DisplayName => "ClientGravityWellTracker";

		public ClientGravityWellTrackerEntity(FrostySdk.Ebx.ClientGravityWellTrackerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

