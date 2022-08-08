using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEEquipmentComponentData))]
	public class MEEquipmentComponent : GameComponent, IEntityData<FrostySdk.Ebx.MEEquipmentComponentData>
	{
		public new FrostySdk.Ebx.MEEquipmentComponentData Data => data as FrostySdk.Ebx.MEEquipmentComponentData;
		public override string DisplayName => "MEEquipmentComponent";

		public MEEquipmentComponent(FrostySdk.Ebx.MEEquipmentComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

