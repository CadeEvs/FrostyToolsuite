using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MovieDebugEntityData))]
	public class MovieDebugEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MovieDebugEntityData>
	{
		public new FrostySdk.Ebx.MovieDebugEntityData Data => data as FrostySdk.Ebx.MovieDebugEntityData;
		public override string DisplayName => "MovieDebug";

		public MovieDebugEntity(FrostySdk.Ebx.MovieDebugEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

