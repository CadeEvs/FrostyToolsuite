using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundPauseEntityData))]
	public class SoundPauseEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundPauseEntityData>
	{
		public new FrostySdk.Ebx.SoundPauseEntityData Data => data as FrostySdk.Ebx.SoundPauseEntityData;
		public override string DisplayName => "SoundPause";

		public SoundPauseEntity(FrostySdk.Ebx.SoundPauseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

