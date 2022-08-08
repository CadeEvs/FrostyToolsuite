using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarTargetEntityData))]
	public class RadarTargetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RadarTargetEntityData>
	{
		public new FrostySdk.Ebx.RadarTargetEntityData Data => data as FrostySdk.Ebx.RadarTargetEntityData;
		public override string DisplayName => "RadarTarget";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RadarTargetEntity(FrostySdk.Ebx.RadarTargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

