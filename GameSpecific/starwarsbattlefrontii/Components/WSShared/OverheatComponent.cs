
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OverheatComponentData))]
	public class OverheatComponent : GameComponent, IEntityData<FrostySdk.Ebx.OverheatComponentData>
	{
		public new FrostySdk.Ebx.OverheatComponentData Data => data as FrostySdk.Ebx.OverheatComponentData;
		public override string DisplayName => "OverheatComponent";

		public OverheatComponent(FrostySdk.Ebx.OverheatComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

