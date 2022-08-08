using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PendingInviteData))]
	public class PendingInvite : LogicEntity, IEntityData<FrostySdk.Ebx.PendingInviteData>
	{
		public new FrostySdk.Ebx.PendingInviteData Data => data as FrostySdk.Ebx.PendingInviteData;
		public override string DisplayName => "PendingInvite";

		public PendingInvite(FrostySdk.Ebx.PendingInviteData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

