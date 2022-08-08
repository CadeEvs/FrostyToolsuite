using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWTwoMovieEntityData))]
	public class BWTwoMovieEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BWTwoMovieEntityData>
	{
		public new FrostySdk.Ebx.BWTwoMovieEntityData Data => data as FrostySdk.Ebx.BWTwoMovieEntityData;
		public override string DisplayName => "BWTwoMovie";

		public BWTwoMovieEntity(FrostySdk.Ebx.BWTwoMovieEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

