using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarGridElementData))]
	public class RadarGridElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.RadarGridElementData>
	{
		public new FrostySdk.Ebx.RadarGridElementData Data => data as FrostySdk.Ebx.RadarGridElementData;
		public override string DisplayName => "RadarGridElement";

		public RadarGridElement(FrostySdk.Ebx.RadarGridElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

