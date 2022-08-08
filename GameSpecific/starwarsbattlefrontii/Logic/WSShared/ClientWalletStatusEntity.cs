using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientWalletStatusEntityData))]
	public class ClientWalletStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientWalletStatusEntityData>
	{
		public new FrostySdk.Ebx.ClientWalletStatusEntityData Data => data as FrostySdk.Ebx.ClientWalletStatusEntityData;
		public override string DisplayName => "ClientWalletStatus";

		public ClientWalletStatusEntity(FrostySdk.Ebx.ClientWalletStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

