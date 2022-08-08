
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WarningSystemComponentData))]
	public class WarningSystemComponent : GameComponent, IEntityData<FrostySdk.Ebx.WarningSystemComponentData>
	{
		public new FrostySdk.Ebx.WarningSystemComponentData Data => data as FrostySdk.Ebx.WarningSystemComponentData;
		public override string DisplayName => "WarningSystemComponent";

		public WarningSystemComponent(FrostySdk.Ebx.WarningSystemComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

