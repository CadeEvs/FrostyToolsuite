using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundContextEntityData))]
	public class SoundContextEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundContextEntityData>
	{
		public new FrostySdk.Ebx.SoundContextEntityData Data => data as FrostySdk.Ebx.SoundContextEntityData;
		public override string DisplayName => "SoundContext";

		public SoundContextEntity(FrostySdk.Ebx.SoundContextEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

