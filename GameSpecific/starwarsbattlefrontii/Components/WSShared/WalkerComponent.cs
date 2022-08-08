
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WalkerComponentData))]
	public class WalkerComponent : GameComponent, IEntityData<FrostySdk.Ebx.WalkerComponentData>
	{
		public new FrostySdk.Ebx.WalkerComponentData Data => data as FrostySdk.Ebx.WalkerComponentData;
		public override string DisplayName => "WalkerComponent";

		public WalkerComponent(FrostySdk.Ebx.WalkerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

