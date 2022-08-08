using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierBodyComponentData))]
	public class SoldierBodyComponent : GameComponent, IEntityData<FrostySdk.Ebx.SoldierBodyComponentData>
	{
		public new FrostySdk.Ebx.SoldierBodyComponentData Data => data as FrostySdk.Ebx.SoldierBodyComponentData;
		public override string DisplayName => "SoldierBodyComponent";

		public SoldierBodyComponent(FrostySdk.Ebx.SoldierBodyComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

