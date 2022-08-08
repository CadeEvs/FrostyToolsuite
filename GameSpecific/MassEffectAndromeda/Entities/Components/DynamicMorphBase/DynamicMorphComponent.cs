using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicMorphComponentData))]
	public class DynamicMorphComponent : GameComponent, IEntityData<FrostySdk.Ebx.DynamicMorphComponentData>
	{
		public new FrostySdk.Ebx.DynamicMorphComponentData Data => data as FrostySdk.Ebx.DynamicMorphComponentData;
		public override string DisplayName => "DynamicMorphComponent";

		public DynamicMorphComponent(FrostySdk.Ebx.DynamicMorphComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

