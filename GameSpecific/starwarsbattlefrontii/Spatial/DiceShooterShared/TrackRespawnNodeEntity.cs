using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TrackRespawnNodeEntityData))]
	public class TrackRespawnNodeEntity : LocatorEntity, IEntityData<FrostySdk.Ebx.TrackRespawnNodeEntityData>
	{
		public new FrostySdk.Ebx.TrackRespawnNodeEntityData Data => data as FrostySdk.Ebx.TrackRespawnNodeEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TrackRespawnNodeEntity(FrostySdk.Ebx.TrackRespawnNodeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

