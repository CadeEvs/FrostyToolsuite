
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AwarenessComponentData))]
	public class AwarenessComponent : GameComponent, IEntityData<FrostySdk.Ebx.AwarenessComponentData>
	{
		public new FrostySdk.Ebx.AwarenessComponentData Data => data as FrostySdk.Ebx.AwarenessComponentData;
		public override string DisplayName => "AwarenessComponent";

		public AwarenessComponent(FrostySdk.Ebx.AwarenessComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

