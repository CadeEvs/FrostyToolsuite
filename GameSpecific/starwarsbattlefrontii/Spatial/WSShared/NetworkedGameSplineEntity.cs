using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NetworkedGameSplineEntityData))]
	public class NetworkedGameSplineEntity : GameSplineEntity, IEntityData<FrostySdk.Ebx.NetworkedGameSplineEntityData>
	{
		public new FrostySdk.Ebx.NetworkedGameSplineEntityData Data => data as FrostySdk.Ebx.NetworkedGameSplineEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public NetworkedGameSplineEntity(FrostySdk.Ebx.NetworkedGameSplineEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

