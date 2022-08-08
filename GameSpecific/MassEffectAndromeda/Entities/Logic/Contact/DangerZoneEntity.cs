using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DangerZoneEntityData))]
	public class DangerZoneEntity : ObstacleControllerEntity, IEntityData<FrostySdk.Ebx.DangerZoneEntityData>
	{
		public new FrostySdk.Ebx.DangerZoneEntityData Data => data as FrostySdk.Ebx.DangerZoneEntityData;
		public override string DisplayName => "DangerZone";

		public DangerZoneEntity(FrostySdk.Ebx.DangerZoneEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

