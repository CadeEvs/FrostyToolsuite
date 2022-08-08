using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PageViewTelemetryEntityData))]
	public class PageViewTelemetryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PageViewTelemetryEntityData>
	{
		public new FrostySdk.Ebx.PageViewTelemetryEntityData Data => data as FrostySdk.Ebx.PageViewTelemetryEntityData;
		public override string DisplayName => "PageViewTelemetry";

		public PageViewTelemetryEntity(FrostySdk.Ebx.PageViewTelemetryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

