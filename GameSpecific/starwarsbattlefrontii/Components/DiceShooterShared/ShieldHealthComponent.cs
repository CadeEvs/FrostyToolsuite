
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShieldHealthComponentData))]
	public class ShieldHealthComponent : GameHealthComponent, IEntityData<FrostySdk.Ebx.ShieldHealthComponentData>
	{
		public new FrostySdk.Ebx.ShieldHealthComponentData Data => data as FrostySdk.Ebx.ShieldHealthComponentData;
		public override string DisplayName => "ShieldHealthComponent";

		public ShieldHealthComponent(FrostySdk.Ebx.ShieldHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

