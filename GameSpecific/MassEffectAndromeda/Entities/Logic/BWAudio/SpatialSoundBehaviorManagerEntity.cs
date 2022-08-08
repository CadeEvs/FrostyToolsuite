using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpatialSoundBehaviorManagerEntityData))]
	public class SpatialSoundBehaviorManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpatialSoundBehaviorManagerEntityData>
	{
		public new FrostySdk.Ebx.SpatialSoundBehaviorManagerEntityData Data => data as FrostySdk.Ebx.SpatialSoundBehaviorManagerEntityData;
		public override string DisplayName => "SpatialSoundBehaviorManager";

		public SpatialSoundBehaviorManagerEntity(FrostySdk.Ebx.SpatialSoundBehaviorManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

