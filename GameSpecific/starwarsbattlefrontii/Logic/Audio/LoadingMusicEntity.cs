using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LoadingMusicEntityData))]
	public class LoadingMusicEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LoadingMusicEntityData>
	{
		public new FrostySdk.Ebx.LoadingMusicEntityData Data => data as FrostySdk.Ebx.LoadingMusicEntityData;
		public override string DisplayName => "LoadingMusic";

		public LoadingMusicEntity(FrostySdk.Ebx.LoadingMusicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

