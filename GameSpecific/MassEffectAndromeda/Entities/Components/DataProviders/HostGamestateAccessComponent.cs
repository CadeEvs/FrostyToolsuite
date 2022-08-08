using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HostGamestateAccessComponentData))]
	public class HostGamestateAccessComponent : GameComponent, IEntityData<FrostySdk.Ebx.HostGamestateAccessComponentData>
	{
		public new FrostySdk.Ebx.HostGamestateAccessComponentData Data => data as FrostySdk.Ebx.HostGamestateAccessComponentData;
		public override string DisplayName => "HostGamestateAccessComponent";

		public HostGamestateAccessComponent(FrostySdk.Ebx.HostGamestateAccessComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

