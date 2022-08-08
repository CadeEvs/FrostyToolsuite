using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UILinkReturnEntityData))]
	public class UILinkReturnEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UILinkReturnEntityData>
	{
		public new FrostySdk.Ebx.UILinkReturnEntityData Data => data as FrostySdk.Ebx.UILinkReturnEntityData;
		public override string DisplayName => "UILinkReturn";

		public UILinkReturnEntity(FrostySdk.Ebx.UILinkReturnEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

