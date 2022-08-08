using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeroNarrativeStateMonitorEntityData))]
	public class HeroNarrativeStateMonitorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HeroNarrativeStateMonitorEntityData>
	{
		public new FrostySdk.Ebx.HeroNarrativeStateMonitorEntityData Data => data as FrostySdk.Ebx.HeroNarrativeStateMonitorEntityData;
		public override string DisplayName => "HeroNarrativeStateMonitor";

		public HeroNarrativeStateMonitorEntity(FrostySdk.Ebx.HeroNarrativeStateMonitorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

