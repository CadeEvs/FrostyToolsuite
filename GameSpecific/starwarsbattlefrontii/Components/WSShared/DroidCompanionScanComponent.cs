
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionScanComponentData))]
	public class DroidCompanionScanComponent : GameComponent, IEntityData<FrostySdk.Ebx.DroidCompanionScanComponentData>
	{
		public new FrostySdk.Ebx.DroidCompanionScanComponentData Data => data as FrostySdk.Ebx.DroidCompanionScanComponentData;
		public override string DisplayName => "DroidCompanionScanComponent";

		public DroidCompanionScanComponent(FrostySdk.Ebx.DroidCompanionScanComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

