using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierGripComponentData))]
	public class SoldierGripComponent : GameComponent, IEntityData<FrostySdk.Ebx.SoldierGripComponentData>
	{
		public new FrostySdk.Ebx.SoldierGripComponentData Data => data as FrostySdk.Ebx.SoldierGripComponentData;
		public override string DisplayName => "SoldierGripComponent";

		public SoldierGripComponent(FrostySdk.Ebx.SoldierGripComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

