
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UnlockComponentData))]
	public class UnlockComponent : GameComponent, IEntityData<FrostySdk.Ebx.UnlockComponentData>
	{
		public new FrostySdk.Ebx.UnlockComponentData Data => data as FrostySdk.Ebx.UnlockComponentData;
		public override string DisplayName => "UnlockComponent";

		public UnlockComponent(FrostySdk.Ebx.UnlockComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

