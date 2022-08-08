using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticMorphGeneratorComponentData))]
	public class StaticMorphGeneratorComponent : GameComponent, IEntityData<FrostySdk.Ebx.StaticMorphGeneratorComponentData>
	{
		public new FrostySdk.Ebx.StaticMorphGeneratorComponentData Data => data as FrostySdk.Ebx.StaticMorphGeneratorComponentData;
		public override string DisplayName => "StaticMorphGeneratorComponent";

		public StaticMorphGeneratorComponent(FrostySdk.Ebx.StaticMorphGeneratorComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

