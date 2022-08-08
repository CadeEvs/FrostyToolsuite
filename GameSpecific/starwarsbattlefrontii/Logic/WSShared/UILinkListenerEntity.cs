using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UILinkListenerEntityData))]
	public class UILinkListenerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UILinkListenerEntityData>
	{
		public new FrostySdk.Ebx.UILinkListenerEntityData Data => data as FrostySdk.Ebx.UILinkListenerEntityData;
		public override string DisplayName => "UILinkListener";

		public UILinkListenerEntity(FrostySdk.Ebx.UILinkListenerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

