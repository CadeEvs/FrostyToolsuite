using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotControllerActionComponentData))]
	public class CinebotControllerActionComponent : GameComponent, IEntityData<FrostySdk.Ebx.CinebotControllerActionComponentData>
	{
		public new FrostySdk.Ebx.CinebotControllerActionComponentData Data => data as FrostySdk.Ebx.CinebotControllerActionComponentData;
		public override string DisplayName => "CinebotControllerActionComponent";

		public CinebotControllerActionComponent(FrostySdk.Ebx.CinebotControllerActionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

