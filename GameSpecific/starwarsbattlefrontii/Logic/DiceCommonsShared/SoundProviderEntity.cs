using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundProviderEntityData))]
	public class SoundProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundProviderEntityData>
	{
		public new FrostySdk.Ebx.SoundProviderEntityData Data => data as FrostySdk.Ebx.SoundProviderEntityData;
		public override string DisplayName => "SoundProvider";

		public SoundProviderEntity(FrostySdk.Ebx.SoundProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

