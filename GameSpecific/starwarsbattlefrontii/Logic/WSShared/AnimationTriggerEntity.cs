using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimationTriggerEntityData))]
	public class AnimationTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AnimationTriggerEntityData>
	{
		public new FrostySdk.Ebx.AnimationTriggerEntityData Data => data as FrostySdk.Ebx.AnimationTriggerEntityData;
		public override string DisplayName => "AnimationTrigger";

		public AnimationTriggerEntity(FrostySdk.Ebx.AnimationTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

