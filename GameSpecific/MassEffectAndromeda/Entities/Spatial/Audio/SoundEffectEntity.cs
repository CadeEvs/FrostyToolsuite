using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundEffectEntityData))]
	public class SoundEffectEntity : ChildEffectEntity, IEntityData<FrostySdk.Ebx.SoundEffectEntityData>
	{
		public new FrostySdk.Ebx.SoundEffectEntityData Data => data as FrostySdk.Ebx.SoundEffectEntityData;

		public SoundEffectEntity(FrostySdk.Ebx.SoundEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

