using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarTargetGameStateEntityData))]
	public class RadarTargetGameStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RadarTargetGameStateEntityData>
	{
		public new FrostySdk.Ebx.RadarTargetGameStateEntityData Data => data as FrostySdk.Ebx.RadarTargetGameStateEntityData;
		public override string DisplayName => "RadarTargetGameState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RadarTargetGameStateEntity(FrostySdk.Ebx.RadarTargetGameStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

