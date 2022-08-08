using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SystemNavigationComponentData))]
	public class SystemNavigationComponent : GameComponent, IEntityData<FrostySdk.Ebx.SystemNavigationComponentData>
	{
		public new FrostySdk.Ebx.SystemNavigationComponentData Data => data as FrostySdk.Ebx.SystemNavigationComponentData;
		public override string DisplayName => "SystemNavigationComponent";

		public SystemNavigationComponent(FrostySdk.Ebx.SystemNavigationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

