using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarTargetToServerEntityData))]
	public class RadarTargetToServerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RadarTargetToServerEntityData>
	{
		public new FrostySdk.Ebx.RadarTargetToServerEntityData Data => data as FrostySdk.Ebx.RadarTargetToServerEntityData;
		public override string DisplayName => "RadarTargetToServer";

		public RadarTargetToServerEntity(FrostySdk.Ebx.RadarTargetToServerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

