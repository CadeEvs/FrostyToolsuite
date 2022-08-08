
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticMorphComponentData))]
	public class StaticMorphComponent : GameComponent, IEntityData<FrostySdk.Ebx.StaticMorphComponentData>
	{
		public new FrostySdk.Ebx.StaticMorphComponentData Data => data as FrostySdk.Ebx.StaticMorphComponentData;
		public override string DisplayName => "StaticMorphComponent";

		public StaticMorphComponent(FrostySdk.Ebx.StaticMorphComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

