using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientCamshotComponentData))]
	public class ClientCamshotComponent : GameComponent, IEntityData<FrostySdk.Ebx.ClientCamshotComponentData>
	{
		public new FrostySdk.Ebx.ClientCamshotComponentData Data => data as FrostySdk.Ebx.ClientCamshotComponentData;
		public override string DisplayName => "ClientCamshotComponent";

		public ClientCamshotComponent(FrostySdk.Ebx.ClientCamshotComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

