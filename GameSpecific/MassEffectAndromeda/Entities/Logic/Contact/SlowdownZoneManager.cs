using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SlowdownZoneManagerData))]
	public class SlowdownZoneManager : LogicEntity, IEntityData<FrostySdk.Ebx.SlowdownZoneManagerData>
	{
		public new FrostySdk.Ebx.SlowdownZoneManagerData Data => data as FrostySdk.Ebx.SlowdownZoneManagerData;
		public override string DisplayName => "SlowdownZoneManager";

		public SlowdownZoneManager(FrostySdk.Ebx.SlowdownZoneManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

