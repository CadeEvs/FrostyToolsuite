using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarTargetTagsData))]
	public class RadarTargetTags : LogicEntity, IEntityData<FrostySdk.Ebx.RadarTargetTagsData>
	{
		public new FrostySdk.Ebx.RadarTargetTagsData Data => data as FrostySdk.Ebx.RadarTargetTagsData;
		public override string DisplayName => "RadarTargetTags";

		public RadarTargetTags(FrostySdk.Ebx.RadarTargetTagsData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

