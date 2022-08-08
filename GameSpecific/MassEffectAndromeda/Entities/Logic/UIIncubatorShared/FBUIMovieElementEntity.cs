using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FBUIMovieElementEntityData))]
	public class FBUIMovieElementEntity : UIElementEntity, IEntityData<FrostySdk.Ebx.FBUIMovieElementEntityData>
	{
		public new FrostySdk.Ebx.FBUIMovieElementEntityData Data => data as FrostySdk.Ebx.FBUIMovieElementEntityData;
		public override string DisplayName => "FBUIMovieElement";

		public FBUIMovieElementEntity(FrostySdk.Ebx.FBUIMovieElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

