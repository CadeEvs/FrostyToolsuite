
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionLinkComponentData))]
	public class DroidCompanionLinkComponent : GameComponent, IEntityData<FrostySdk.Ebx.DroidCompanionLinkComponentData>
	{
		public new FrostySdk.Ebx.DroidCompanionLinkComponentData Data => data as FrostySdk.Ebx.DroidCompanionLinkComponentData;
		public override string DisplayName => "DroidCompanionLinkComponent";

		public DroidCompanionLinkComponent(FrostySdk.Ebx.DroidCompanionLinkComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

