using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundListenerEntityData))]
	public class SoundListenerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundListenerEntityData>
	{
		public new FrostySdk.Ebx.SoundListenerEntityData Data => data as FrostySdk.Ebx.SoundListenerEntityData;
		public override string DisplayName => "SoundListener";

		public SoundListenerEntity(FrostySdk.Ebx.SoundListenerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

