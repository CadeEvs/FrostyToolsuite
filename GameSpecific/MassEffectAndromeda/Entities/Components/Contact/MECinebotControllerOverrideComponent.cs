using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECinebotControllerOverrideComponentData))]
	public class MECinebotControllerOverrideComponent : CinebotControllerOverrideComponent, IEntityData<FrostySdk.Ebx.MECinebotControllerOverrideComponentData>
	{
		public new FrostySdk.Ebx.MECinebotControllerOverrideComponentData Data => data as FrostySdk.Ebx.MECinebotControllerOverrideComponentData;
		public override string DisplayName => "MECinebotControllerOverrideComponent";

		public MECinebotControllerOverrideComponent(FrostySdk.Ebx.MECinebotControllerOverrideComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

