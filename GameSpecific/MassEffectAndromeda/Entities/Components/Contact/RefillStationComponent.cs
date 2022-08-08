using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RefillStationComponentData))]
	public class RefillStationComponent : GameComponent, IEntityData<FrostySdk.Ebx.RefillStationComponentData>
	{
		public new FrostySdk.Ebx.RefillStationComponentData Data => data as FrostySdk.Ebx.RefillStationComponentData;
		public override string DisplayName => "RefillStationComponent";

		public RefillStationComponent(FrostySdk.Ebx.RefillStationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

