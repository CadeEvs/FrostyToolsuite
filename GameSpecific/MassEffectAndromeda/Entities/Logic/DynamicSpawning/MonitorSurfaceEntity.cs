using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MonitorSurfaceEntityData))]
	public class MonitorSurfaceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MonitorSurfaceEntityData>
	{
		public new FrostySdk.Ebx.MonitorSurfaceEntityData Data => data as FrostySdk.Ebx.MonitorSurfaceEntityData;
		public override string DisplayName => "MonitorSurface";

		public MonitorSurfaceEntity(FrostySdk.Ebx.MonitorSurfaceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

