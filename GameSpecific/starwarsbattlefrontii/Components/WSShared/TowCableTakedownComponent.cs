
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TowCableTakedownComponentData))]
	public class TowCableTakedownComponent : GameComponent, IEntityData<FrostySdk.Ebx.TowCableTakedownComponentData>
	{
		public new FrostySdk.Ebx.TowCableTakedownComponentData Data => data as FrostySdk.Ebx.TowCableTakedownComponentData;
		public override string DisplayName => "TowCableTakedownComponent";

		public TowCableTakedownComponent(FrostySdk.Ebx.TowCableTakedownComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

