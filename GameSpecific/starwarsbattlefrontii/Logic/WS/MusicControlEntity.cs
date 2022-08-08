using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MusicControlEntityData))]
	public class MusicControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MusicControlEntityData>
	{
		public new FrostySdk.Ebx.MusicControlEntityData Data => data as FrostySdk.Ebx.MusicControlEntityData;
		public override string DisplayName => "MusicControl";

		public MusicControlEntity(FrostySdk.Ebx.MusicControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

