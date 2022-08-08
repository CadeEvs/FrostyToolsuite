using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AppearanceComponentData))]
	public class AppearanceComponent : GameComponent, IEntityData<FrostySdk.Ebx.AppearanceComponentData>
	{
		public new FrostySdk.Ebx.AppearanceComponentData Data => data as FrostySdk.Ebx.AppearanceComponentData;
		public override string DisplayName => "AppearanceComponent";

		public AppearanceComponent(FrostySdk.Ebx.AppearanceComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

