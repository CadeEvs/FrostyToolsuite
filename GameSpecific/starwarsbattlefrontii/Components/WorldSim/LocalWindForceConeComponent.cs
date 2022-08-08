
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalWindForceConeComponentData))]
	public class LocalWindForceConeComponent : LocalWindForceComponentBase, IEntityData<FrostySdk.Ebx.LocalWindForceConeComponentData>
	{
		public new FrostySdk.Ebx.LocalWindForceConeComponentData Data => data as FrostySdk.Ebx.LocalWindForceConeComponentData;
		public override string DisplayName => "LocalWindForceConeComponent";

		public LocalWindForceConeComponent(FrostySdk.Ebx.LocalWindForceConeComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

