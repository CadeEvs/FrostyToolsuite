
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionHealthComponentData))]
	public class DroidCompanionHealthComponent : ControllableHealthComponent, IEntityData<FrostySdk.Ebx.DroidCompanionHealthComponentData>
	{
		public new FrostySdk.Ebx.DroidCompanionHealthComponentData Data => data as FrostySdk.Ebx.DroidCompanionHealthComponentData;
		public override string DisplayName => "DroidCompanionHealthComponent";

		public DroidCompanionHealthComponent(FrostySdk.Ebx.DroidCompanionHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

