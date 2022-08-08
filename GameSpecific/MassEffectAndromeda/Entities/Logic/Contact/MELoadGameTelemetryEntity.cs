using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MELoadGameTelemetryEntityData))]
	public class MELoadGameTelemetryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MELoadGameTelemetryEntityData>
	{
		public new FrostySdk.Ebx.MELoadGameTelemetryEntityData Data => data as FrostySdk.Ebx.MELoadGameTelemetryEntityData;
		public override string DisplayName => "MELoadGameTelemetry";

		public MELoadGameTelemetryEntity(FrostySdk.Ebx.MELoadGameTelemetryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

