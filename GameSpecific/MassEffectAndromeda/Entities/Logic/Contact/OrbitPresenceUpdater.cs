using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OrbitPresenceUpdaterData))]
	public class OrbitPresenceUpdater : LogicEntity, IEntityData<FrostySdk.Ebx.OrbitPresenceUpdaterData>
	{
		public new FrostySdk.Ebx.OrbitPresenceUpdaterData Data => data as FrostySdk.Ebx.OrbitPresenceUpdaterData;
		public override string DisplayName => "OrbitPresenceUpdater";

		public OrbitPresenceUpdater(FrostySdk.Ebx.OrbitPresenceUpdaterData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

