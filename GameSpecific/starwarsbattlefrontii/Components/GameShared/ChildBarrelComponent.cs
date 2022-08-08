
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChildBarrelComponentData))]
	public class ChildBarrelComponent : ChildComponent, IEntityData<FrostySdk.Ebx.ChildBarrelComponentData>
	{
		public new FrostySdk.Ebx.ChildBarrelComponentData Data => data as FrostySdk.Ebx.ChildBarrelComponentData;
		public override string DisplayName => "ChildBarrelComponent";

		public ChildBarrelComponent(FrostySdk.Ebx.ChildBarrelComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

