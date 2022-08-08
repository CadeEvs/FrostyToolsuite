using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TrackLocalPlayerEntityData))]
	public class TrackLocalPlayerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TrackLocalPlayerEntityData>
	{
		public new FrostySdk.Ebx.TrackLocalPlayerEntityData Data => data as FrostySdk.Ebx.TrackLocalPlayerEntityData;
		public override string DisplayName => "TrackLocalPlayer";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TrackLocalPlayerEntity(FrostySdk.Ebx.TrackLocalPlayerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

