
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierSoundComponentData))]
	public class SoldierSoundComponent : GameComponent, IEntityData<FrostySdk.Ebx.SoldierSoundComponentData>
	{
		public new FrostySdk.Ebx.SoldierSoundComponentData Data => data as FrostySdk.Ebx.SoldierSoundComponentData;
		public override string DisplayName => "SoldierSoundComponent";

		public SoldierSoundComponent(FrostySdk.Ebx.SoldierSoundComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

