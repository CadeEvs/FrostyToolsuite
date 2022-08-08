using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocatorComponentData))]
	public class LocatorComponent : GameComponent, IEntityData<FrostySdk.Ebx.LocatorComponentData>
	{
		public new FrostySdk.Ebx.LocatorComponentData Data => data as FrostySdk.Ebx.LocatorComponentData;
		public override string DisplayName => "LocatorComponent";

		public LocatorComponent(FrostySdk.Ebx.LocatorComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

