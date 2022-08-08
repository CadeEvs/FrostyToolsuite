using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundContextLinkEntityData))]
	public class SoundContextLinkEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundContextLinkEntityData>
	{
		public new FrostySdk.Ebx.SoundContextLinkEntityData Data => data as FrostySdk.Ebx.SoundContextLinkEntityData;
		public override string DisplayName => "SoundContextLink";

		public SoundContextLinkEntity(FrostySdk.Ebx.SoundContextLinkEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

