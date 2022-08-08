using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MiningDroneComponentData))]
	public class MiningDroneComponent : GameComponent, IEntityData<FrostySdk.Ebx.MiningDroneComponentData>
	{
		public new FrostySdk.Ebx.MiningDroneComponentData Data => data as FrostySdk.Ebx.MiningDroneComponentData;
		public override string DisplayName => "MiningDroneComponent";

		public MiningDroneComponent(FrostySdk.Ebx.MiningDroneComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

