
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FilmGrainComponentData))]
	public class FilmGrainComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.FilmGrainComponentData>
	{
		public new FrostySdk.Ebx.FilmGrainComponentData Data => data as FrostySdk.Ebx.FilmGrainComponentData;
		public override string DisplayName => "FilmGrainComponent";

		public FilmGrainComponent(FrostySdk.Ebx.FilmGrainComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

