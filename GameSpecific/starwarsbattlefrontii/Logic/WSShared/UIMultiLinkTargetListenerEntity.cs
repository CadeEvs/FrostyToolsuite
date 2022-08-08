using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIMultiLinkTargetListenerEntityData))]
	public class UIMultiLinkTargetListenerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIMultiLinkTargetListenerEntityData>
	{
		public new FrostySdk.Ebx.UIMultiLinkTargetListenerEntityData Data => data as FrostySdk.Ebx.UIMultiLinkTargetListenerEntityData;
		public override string DisplayName => "UIMultiLinkTargetListener";

		public UIMultiLinkTargetListenerEntity(FrostySdk.Ebx.UIMultiLinkTargetListenerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

