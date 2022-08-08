using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GotoUILinkTargetEntityData))]
	public class GotoUILinkTargetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GotoUILinkTargetEntityData>
	{
		public new FrostySdk.Ebx.GotoUILinkTargetEntityData Data => data as FrostySdk.Ebx.GotoUILinkTargetEntityData;
		public override string DisplayName => "GotoUILinkTarget";

		public GotoUILinkTargetEntity(FrostySdk.Ebx.GotoUILinkTargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

