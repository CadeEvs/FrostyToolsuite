using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UILinkTargetEntityData))]
	public class UILinkTargetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UILinkTargetEntityData>
	{
		public new FrostySdk.Ebx.UILinkTargetEntityData Data => data as FrostySdk.Ebx.UILinkTargetEntityData;
		public override string DisplayName => "UILinkTarget";

		public UILinkTargetEntity(FrostySdk.Ebx.UILinkTargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

