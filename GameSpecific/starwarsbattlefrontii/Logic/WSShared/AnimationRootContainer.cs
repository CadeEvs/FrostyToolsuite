using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimationRootContainerData))]
	public class AnimationRootContainer : UIAnimationNode, IEntityData<FrostySdk.Ebx.AnimationRootContainerData>
	{
		public new FrostySdk.Ebx.AnimationRootContainerData Data => data as FrostySdk.Ebx.AnimationRootContainerData;
		public override string DisplayName => "AnimationRootContainer";

		public AnimationRootContainer(FrostySdk.Ebx.AnimationRootContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

