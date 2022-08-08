using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GotoDynamicUILinkTargetEntityData))]
	public class GotoDynamicUILinkTargetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GotoDynamicUILinkTargetEntityData>
	{
		public new FrostySdk.Ebx.GotoDynamicUILinkTargetEntityData Data => data as FrostySdk.Ebx.GotoDynamicUILinkTargetEntityData;
		public override string DisplayName => "GotoDynamicUILinkTarget";

		public GotoDynamicUILinkTargetEntity(FrostySdk.Ebx.GotoDynamicUILinkTargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

