
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TwoPartComponentData))]
	public class TwoPartComponent : GameComponent, IEntityData<FrostySdk.Ebx.TwoPartComponentData>
	{
		public new FrostySdk.Ebx.TwoPartComponentData Data => data as FrostySdk.Ebx.TwoPartComponentData;
		public override string DisplayName => "TwoPartComponent";

		public TwoPartComponent(FrostySdk.Ebx.TwoPartComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

