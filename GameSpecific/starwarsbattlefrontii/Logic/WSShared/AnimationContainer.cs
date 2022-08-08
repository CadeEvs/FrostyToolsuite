using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimationContainerData))]
	public class AnimationContainer : UIAnimationNode, IEntityData<FrostySdk.Ebx.AnimationContainerData>
	{
		public new FrostySdk.Ebx.AnimationContainerData Data => data as FrostySdk.Ebx.AnimationContainerData;
		public override string DisplayName => "AnimationContainer";

		public AnimationContainer(FrostySdk.Ebx.AnimationContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

