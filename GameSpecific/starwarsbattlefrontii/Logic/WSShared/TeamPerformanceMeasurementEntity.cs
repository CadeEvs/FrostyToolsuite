using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TeamPerformanceMeasurementEntityData))]
	public class TeamPerformanceMeasurementEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TeamPerformanceMeasurementEntityData>
	{
		public new FrostySdk.Ebx.TeamPerformanceMeasurementEntityData Data => data as FrostySdk.Ebx.TeamPerformanceMeasurementEntityData;
		public override string DisplayName => "TeamPerformanceMeasurement";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TeamPerformanceMeasurementEntity(FrostySdk.Ebx.TeamPerformanceMeasurementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

