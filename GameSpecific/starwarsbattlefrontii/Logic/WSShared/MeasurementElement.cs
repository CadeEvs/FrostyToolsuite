using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MeasurementElementData))]
	public class MeasurementElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.MeasurementElementData>
	{
		public new FrostySdk.Ebx.MeasurementElementData Data => data as FrostySdk.Ebx.MeasurementElementData;
		public override string DisplayName => "MeasurementElement";

		public MeasurementElement(FrostySdk.Ebx.MeasurementElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

