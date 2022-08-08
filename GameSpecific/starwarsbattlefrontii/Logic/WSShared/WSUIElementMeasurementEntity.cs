using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSUIElementMeasurementEntityData))]
	public class WSUIElementMeasurementEntity : WSUIElementEntity, IEntityData<FrostySdk.Ebx.WSUIElementMeasurementEntityData>
	{
		public new FrostySdk.Ebx.WSUIElementMeasurementEntityData Data => data as FrostySdk.Ebx.WSUIElementMeasurementEntityData;
		public override string DisplayName => "WSUIElementMeasurement";

		public WSUIElementMeasurementEntity(FrostySdk.Ebx.WSUIElementMeasurementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

