using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIAnimationNodeData))]
	public class UIAnimationNode : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.UIAnimationNodeData>
	{
		public new FrostySdk.Ebx.UIAnimationNodeData Data => data as FrostySdk.Ebx.UIAnimationNodeData;
		public override string DisplayName => "UIAnimationNode";

		public UIAnimationNode(FrostySdk.Ebx.UIAnimationNodeData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

