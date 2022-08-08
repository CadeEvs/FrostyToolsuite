using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayVideoEntityData))]
	public class PlayVideoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayVideoEntityData>
	{
		public new FrostySdk.Ebx.PlayVideoEntityData Data => data as FrostySdk.Ebx.PlayVideoEntityData;
		public override string DisplayName => "PlayVideo";

		public PlayVideoEntity(FrostySdk.Ebx.PlayVideoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

