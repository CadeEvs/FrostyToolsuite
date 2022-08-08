using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarCaptureAreaElementData))]
	public class RadarCaptureAreaElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.RadarCaptureAreaElementData>
	{
		public new FrostySdk.Ebx.RadarCaptureAreaElementData Data => data as FrostySdk.Ebx.RadarCaptureAreaElementData;
		public override string DisplayName => "RadarCaptureAreaElement";

		public RadarCaptureAreaElement(FrostySdk.Ebx.RadarCaptureAreaElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

