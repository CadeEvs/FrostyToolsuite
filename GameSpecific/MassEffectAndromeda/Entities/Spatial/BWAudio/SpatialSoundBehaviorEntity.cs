using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpatialSoundBehaviorEntityData))]
	public class SpatialSoundBehaviorEntity : LocatorEntity, IEntityData<FrostySdk.Ebx.SpatialSoundBehaviorEntityData>
	{
		public new FrostySdk.Ebx.SpatialSoundBehaviorEntityData Data => data as FrostySdk.Ebx.SpatialSoundBehaviorEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpatialSoundBehaviorEntity(FrostySdk.Ebx.SpatialSoundBehaviorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

