using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PurchaseMultiplayerPrivilegeEntityData))]
	public class PurchaseMultiplayerPrivilegeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PurchaseMultiplayerPrivilegeEntityData>
	{
		public new FrostySdk.Ebx.PurchaseMultiplayerPrivilegeEntityData Data => data as FrostySdk.Ebx.PurchaseMultiplayerPrivilegeEntityData;
		public override string DisplayName => "PurchaseMultiplayerPrivilege";

		public PurchaseMultiplayerPrivilegeEntity(FrostySdk.Ebx.PurchaseMultiplayerPrivilegeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

