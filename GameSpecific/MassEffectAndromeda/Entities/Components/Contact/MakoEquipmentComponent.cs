using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MakoEquipmentComponentData))]
	public class MakoEquipmentComponent : GameComponent, IEntityData<FrostySdk.Ebx.MakoEquipmentComponentData>
	{
		public new FrostySdk.Ebx.MakoEquipmentComponentData Data => data as FrostySdk.Ebx.MakoEquipmentComponentData;
		public override string DisplayName => "MakoEquipmentComponent";

		public MakoEquipmentComponent(FrostySdk.Ebx.MakoEquipmentComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

