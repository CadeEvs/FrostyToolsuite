
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MissileHealthComponentData))]
	public class MissileHealthComponent : GameHealthComponent, IEntityData<FrostySdk.Ebx.MissileHealthComponentData>
	{
		public new FrostySdk.Ebx.MissileHealthComponentData Data => data as FrostySdk.Ebx.MissileHealthComponentData;
		public override string DisplayName => "MissileHealthComponent";

		public MissileHealthComponent(FrostySdk.Ebx.MissileHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

