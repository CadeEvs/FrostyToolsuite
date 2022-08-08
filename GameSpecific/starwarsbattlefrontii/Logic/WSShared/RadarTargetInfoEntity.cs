using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarTargetInfoEntityData))]
	public class RadarTargetInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RadarTargetInfoEntityData>
	{
		public new FrostySdk.Ebx.RadarTargetInfoEntityData Data => data as FrostySdk.Ebx.RadarTargetInfoEntityData;
		public override string DisplayName => "RadarTargetInfo";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RadarTargetInfoEntity(FrostySdk.Ebx.RadarTargetInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

