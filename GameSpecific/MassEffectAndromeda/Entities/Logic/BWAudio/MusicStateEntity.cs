using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MusicStateEntityData))]
	public class MusicStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MusicStateEntityData>
	{
		public new FrostySdk.Ebx.MusicStateEntityData Data => data as FrostySdk.Ebx.MusicStateEntityData;
		public override string DisplayName => "MusicState";

		public MusicStateEntity(FrostySdk.Ebx.MusicStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

