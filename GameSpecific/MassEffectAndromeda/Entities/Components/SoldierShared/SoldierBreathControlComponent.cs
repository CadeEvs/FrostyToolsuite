using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierBreathControlComponentData))]
	public class SoldierBreathControlComponent : GameComponent, IEntityData<FrostySdk.Ebx.SoldierBreathControlComponentData>
	{
		public new FrostySdk.Ebx.SoldierBreathControlComponentData Data => data as FrostySdk.Ebx.SoldierBreathControlComponentData;
		public override string DisplayName => "SoldierBreathControlComponent";

		public SoldierBreathControlComponent(FrostySdk.Ebx.SoldierBreathControlComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

