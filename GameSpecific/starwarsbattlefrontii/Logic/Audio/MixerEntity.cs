using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MixerEntityData))]
	public class MixerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MixerEntityData>
	{
		public new FrostySdk.Ebx.MixerEntityData Data => data as FrostySdk.Ebx.MixerEntityData;
		public override string DisplayName => "Mixer";

		public MixerEntity(FrostySdk.Ebx.MixerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

