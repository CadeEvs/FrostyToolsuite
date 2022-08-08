using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExecutingMEStatusEffectData))]
	public class ExecutingMEStatusEffect : BWExecutingTimeline, IEntityData<FrostySdk.Ebx.ExecutingMEStatusEffectData>
	{
		public new FrostySdk.Ebx.ExecutingMEStatusEffectData Data => data as FrostySdk.Ebx.ExecutingMEStatusEffectData;
		public override string DisplayName => "ExecutingMEStatusEffect";

		public ExecutingMEStatusEffect(FrostySdk.Ebx.ExecutingMEStatusEffectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

