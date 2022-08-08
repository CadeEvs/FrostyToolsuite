
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OcclutionQueryComponentData))]
	public class OcclutionQueryComponent : GameComponent, IEntityData<FrostySdk.Ebx.OcclutionQueryComponentData>
	{
		public new FrostySdk.Ebx.OcclutionQueryComponentData Data => data as FrostySdk.Ebx.OcclutionQueryComponentData;
		public override string DisplayName => "OcclutionQueryComponent";

		public OcclutionQueryComponent(FrostySdk.Ebx.OcclutionQueryComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

