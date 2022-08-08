using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSSnowglobeTrackerEntityData))]
	public class WSSnowglobeTrackerEntity : SnowglobeTrackerEntity, IEntityData<FrostySdk.Ebx.WSSnowglobeTrackerEntityData>
	{
		public new FrostySdk.Ebx.WSSnowglobeTrackerEntityData Data => data as FrostySdk.Ebx.WSSnowglobeTrackerEntityData;
		public override string DisplayName => "WSSnowglobeTracker";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSSnowglobeTrackerEntity(FrostySdk.Ebx.WSSnowglobeTrackerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

