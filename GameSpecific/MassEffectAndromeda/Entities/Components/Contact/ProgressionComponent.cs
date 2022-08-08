using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProgressionComponentData))]
	public class ProgressionComponent : GameComponent, IEntityData<FrostySdk.Ebx.ProgressionComponentData>
	{
		public new FrostySdk.Ebx.ProgressionComponentData Data => data as FrostySdk.Ebx.ProgressionComponentData;
		public override string DisplayName => "ProgressionComponent";

		public ProgressionComponent(FrostySdk.Ebx.ProgressionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

