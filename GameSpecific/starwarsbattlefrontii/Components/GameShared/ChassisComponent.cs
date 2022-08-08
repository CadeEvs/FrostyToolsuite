
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChassisComponentData))]
	public class ChassisComponent : PartComponent, IEntityData<FrostySdk.Ebx.ChassisComponentData>
	{
		public new FrostySdk.Ebx.ChassisComponentData Data => data as FrostySdk.Ebx.ChassisComponentData;
		public override string DisplayName => "ChassisComponent";

		public ChassisComponent(FrostySdk.Ebx.ChassisComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

