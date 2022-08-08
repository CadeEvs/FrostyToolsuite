using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimationEventDelayEntityData))]
	public class AnimationEventDelayEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AnimationEventDelayEntityData>
	{
		public new FrostySdk.Ebx.AnimationEventDelayEntityData Data => data as FrostySdk.Ebx.AnimationEventDelayEntityData;
		public override string DisplayName => "AnimationEventDelay";

		public AnimationEventDelayEntity(FrostySdk.Ebx.AnimationEventDelayEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

