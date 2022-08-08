
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LightComponentData))]
	public class LightComponent : GameComponent, IEntityData<FrostySdk.Ebx.LightComponentData>
	{
		public new FrostySdk.Ebx.LightComponentData Data => data as FrostySdk.Ebx.LightComponentData;
		public override string DisplayName => "LightComponent";

		public LightComponent(FrostySdk.Ebx.LightComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

