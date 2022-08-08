
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AffectorComponentData))]
	public class AffectorComponent : GameComponent, IEntityData<FrostySdk.Ebx.AffectorComponentData>
	{
		public new FrostySdk.Ebx.AffectorComponentData Data => data as FrostySdk.Ebx.AffectorComponentData;
		public override string DisplayName => "AffectorComponent";

		public AffectorComponent(FrostySdk.Ebx.AffectorComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

