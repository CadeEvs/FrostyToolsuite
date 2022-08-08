using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SnowglobeTrackerEntityData))]
	public class SnowglobeTrackerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SnowglobeTrackerEntityData>
	{
		public new FrostySdk.Ebx.SnowglobeTrackerEntityData Data => data as FrostySdk.Ebx.SnowglobeTrackerEntityData;
		public override string DisplayName => "SnowglobeTracker";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SnowglobeTrackerEntity(FrostySdk.Ebx.SnowglobeTrackerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

