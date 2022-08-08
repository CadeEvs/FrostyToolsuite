using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TelemetryManagerData))]
	public class TelemetryManager : LogicEntity, IEntityData<FrostySdk.Ebx.TelemetryManagerData>
	{
		public new FrostySdk.Ebx.TelemetryManagerData Data => data as FrostySdk.Ebx.TelemetryManagerData;
		public override string DisplayName => "TelemetryManager";

		public TelemetryManager(FrostySdk.Ebx.TelemetryManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

