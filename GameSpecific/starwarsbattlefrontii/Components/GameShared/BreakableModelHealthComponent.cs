
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BreakableModelHealthComponentData))]
	public class BreakableModelHealthComponent : GameHealthComponent, IEntityData<FrostySdk.Ebx.BreakableModelHealthComponentData>
	{
		public new FrostySdk.Ebx.BreakableModelHealthComponentData Data => data as FrostySdk.Ebx.BreakableModelHealthComponentData;
		public override string DisplayName => "BreakableModelHealthComponent";

		public BreakableModelHealthComponent(FrostySdk.Ebx.BreakableModelHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

