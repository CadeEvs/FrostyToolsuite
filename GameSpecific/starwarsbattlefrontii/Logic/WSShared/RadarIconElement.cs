using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarIconElementData))]
	public class RadarIconElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.RadarIconElementData>
	{
		public new FrostySdk.Ebx.RadarIconElementData Data => data as FrostySdk.Ebx.RadarIconElementData;
		public override string DisplayName => "RadarIconElement";

		public RadarIconElement(FrostySdk.Ebx.RadarIconElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

