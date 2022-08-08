using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FilmicEffectsComponentData))]
	public class FilmicEffectsComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.FilmicEffectsComponentData>
	{
		public new FrostySdk.Ebx.FilmicEffectsComponentData Data => data as FrostySdk.Ebx.FilmicEffectsComponentData;
		public override string DisplayName => "FilmicEffectsComponent";

		public FilmicEffectsComponent(FrostySdk.Ebx.FilmicEffectsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

