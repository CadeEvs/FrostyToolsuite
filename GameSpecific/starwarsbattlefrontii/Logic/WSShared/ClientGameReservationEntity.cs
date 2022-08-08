using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientGameReservationEntityData))]
	public class ClientGameReservationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientGameReservationEntityData>
	{
		public new FrostySdk.Ebx.ClientGameReservationEntityData Data => data as FrostySdk.Ebx.ClientGameReservationEntityData;
		public override string DisplayName => "ClientGameReservation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientGameReservationEntity(FrostySdk.Ebx.ClientGameReservationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

