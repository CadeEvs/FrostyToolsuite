using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DetachedSoundListenerEntityData))]
	public class DetachedSoundListenerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DetachedSoundListenerEntityData>
	{
		public new FrostySdk.Ebx.DetachedSoundListenerEntityData Data => data as FrostySdk.Ebx.DetachedSoundListenerEntityData;
		public override string DisplayName => "DetachedSoundListener";

		public DetachedSoundListenerEntity(FrostySdk.Ebx.DetachedSoundListenerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

