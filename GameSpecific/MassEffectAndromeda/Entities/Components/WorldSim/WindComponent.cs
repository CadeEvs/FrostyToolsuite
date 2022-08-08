using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WindComponentData))]
	public class WindComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.WindComponentData>
	{
		public new FrostySdk.Ebx.WindComponentData Data => data as FrostySdk.Ebx.WindComponentData;
		public override string DisplayName => "WindComponent";

		public WindComponent(FrostySdk.Ebx.WindComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

