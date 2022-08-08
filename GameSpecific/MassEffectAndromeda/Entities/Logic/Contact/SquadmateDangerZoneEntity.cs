using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadmateDangerZoneEntityData))]
	public class SquadmateDangerZoneEntity : DangerZoneEntity, IEntityData<FrostySdk.Ebx.SquadmateDangerZoneEntityData>
	{
		public new FrostySdk.Ebx.SquadmateDangerZoneEntityData Data => data as FrostySdk.Ebx.SquadmateDangerZoneEntityData;
		public override string DisplayName => "SquadmateDangerZone";

		public SquadmateDangerZoneEntity(FrostySdk.Ebx.SquadmateDangerZoneEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

