
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticModelNetworkDestructionComponentData))]
	public class StaticModelNetworkDestructionComponent : GameComponent, IEntityData<FrostySdk.Ebx.StaticModelNetworkDestructionComponentData>
	{
		public new FrostySdk.Ebx.StaticModelNetworkDestructionComponentData Data => data as FrostySdk.Ebx.StaticModelNetworkDestructionComponentData;
		public override string DisplayName => "StaticModelNetworkDestructionComponent";

		public StaticModelNetworkDestructionComponent(FrostySdk.Ebx.StaticModelNetworkDestructionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

