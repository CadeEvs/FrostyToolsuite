using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TrackPlayerEntityData))]
	public class TrackPlayerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.TrackPlayerEntityData>
	{
		public new FrostySdk.Ebx.TrackPlayerEntityData Data => data as FrostySdk.Ebx.TrackPlayerEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TrackPlayerEntity(FrostySdk.Ebx.TrackPlayerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

