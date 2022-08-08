using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWMovieEntityData))]
	public class BWMovieEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BWMovieEntityData>
	{
		public new FrostySdk.Ebx.BWMovieEntityData Data => data as FrostySdk.Ebx.BWMovieEntityData;
		public override string DisplayName => "BWMovie";

		public BWMovieEntity(FrostySdk.Ebx.BWMovieEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

