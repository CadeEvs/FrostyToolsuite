using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MusicInstanceEntityData))]
	public class MusicInstanceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MusicInstanceEntityData>
	{
		public new FrostySdk.Ebx.MusicInstanceEntityData Data => data as FrostySdk.Ebx.MusicInstanceEntityData;
		public override string DisplayName => "MusicInstance";

		public MusicInstanceEntity(FrostySdk.Ebx.MusicInstanceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

