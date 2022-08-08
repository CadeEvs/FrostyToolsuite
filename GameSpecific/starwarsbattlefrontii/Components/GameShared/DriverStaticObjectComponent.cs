
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DriverStaticObjectComponentData))]
	public class DriverStaticObjectComponent : DriverComponent, IEntityData<FrostySdk.Ebx.DriverStaticObjectComponentData>
	{
		public new FrostySdk.Ebx.DriverStaticObjectComponentData Data => data as FrostySdk.Ebx.DriverStaticObjectComponentData;
		public override string DisplayName => "DriverStaticObjectComponent";

		public DriverStaticObjectComponent(FrostySdk.Ebx.DriverStaticObjectComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

