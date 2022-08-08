
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SnowglobeComponentData))]
	public class SnowglobeComponent : Component, IEntityData<FrostySdk.Ebx.SnowglobeComponentData>
	{
		public new FrostySdk.Ebx.SnowglobeComponentData Data => data as FrostySdk.Ebx.SnowglobeComponentData;
		public override string DisplayName => "SnowglobeComponent";

		public SnowglobeComponent(FrostySdk.Ebx.SnowglobeComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

