
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarfighterHealthComponentData))]
	public class StarfighterHealthComponent : WSVehicleHealthComponent, IEntityData<FrostySdk.Ebx.StarfighterHealthComponentData>
	{
		public new FrostySdk.Ebx.StarfighterHealthComponentData Data => data as FrostySdk.Ebx.StarfighterHealthComponentData;
		public override string DisplayName => "StarfighterHealthComponent";

		public StarfighterHealthComponent(FrostySdk.Ebx.StarfighterHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

