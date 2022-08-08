using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SlowdownZoneData))]
	public class SlowdownZone : LogicEntity, IEntityData<FrostySdk.Ebx.SlowdownZoneData>
	{
		public new FrostySdk.Ebx.SlowdownZoneData Data => data as FrostySdk.Ebx.SlowdownZoneData;
		public override string DisplayName => "SlowdownZone";

		public SlowdownZone(FrostySdk.Ebx.SlowdownZoneData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

