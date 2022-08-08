using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundBehaviorEntityData))]
	public class SoundBehaviorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundBehaviorEntityData>
	{
		public new FrostySdk.Ebx.SoundBehaviorEntityData Data => data as FrostySdk.Ebx.SoundBehaviorEntityData;
		public override string DisplayName => "SoundBehavior";

		public SoundBehaviorEntity(FrostySdk.Ebx.SoundBehaviorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

