using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SocketComponentData))]
	public class SocketComponent : GameComponent, IEntityData<FrostySdk.Ebx.SocketComponentData>
	{
		public new FrostySdk.Ebx.SocketComponentData Data => data as FrostySdk.Ebx.SocketComponentData;
		public override string DisplayName => "SocketComponent";

		public SocketComponent(FrostySdk.Ebx.SocketComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

