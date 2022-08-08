using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ManDownComponentData))]
	public class ManDownComponent : GameComponent, IEntityData<FrostySdk.Ebx.ManDownComponentData>
	{
		public new FrostySdk.Ebx.ManDownComponentData Data => data as FrostySdk.Ebx.ManDownComponentData;
		public override string DisplayName => "ManDownComponent";

		public ManDownComponent(FrostySdk.Ebx.ManDownComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

