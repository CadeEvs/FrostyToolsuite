using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MovieEntityData))]
	public class MovieEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MovieEntityData>
	{
		public new FrostySdk.Ebx.MovieEntityData Data => data as FrostySdk.Ebx.MovieEntityData;
		public override string DisplayName => "Movie";

		public MovieEntity(FrostySdk.Ebx.MovieEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

