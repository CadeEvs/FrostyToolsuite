using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotControllerOverrideComponentData))]
	public class CinebotControllerOverrideComponent : GameComponent, IEntityData<FrostySdk.Ebx.CinebotControllerOverrideComponentData>
	{
		public new FrostySdk.Ebx.CinebotControllerOverrideComponentData Data => data as FrostySdk.Ebx.CinebotControllerOverrideComponentData;
		public override string DisplayName => "CinebotControllerOverrideComponent";

		public CinebotControllerOverrideComponent(FrostySdk.Ebx.CinebotControllerOverrideComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

