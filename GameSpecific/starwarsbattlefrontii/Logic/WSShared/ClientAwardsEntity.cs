using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientAwardsEntityData))]
	public class ClientAwardsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientAwardsEntityData>
	{
		public new FrostySdk.Ebx.ClientAwardsEntityData Data => data as FrostySdk.Ebx.ClientAwardsEntityData;
		public override string DisplayName => "ClientAwards";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientAwardsEntity(FrostySdk.Ebx.ClientAwardsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

