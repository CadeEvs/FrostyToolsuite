
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DefaultHealthComponentData))]
	public class DefaultHealthComponent : HealthComponent, IEntityData<FrostySdk.Ebx.DefaultHealthComponentData>
	{
		public new FrostySdk.Ebx.DefaultHealthComponentData Data => data as FrostySdk.Ebx.DefaultHealthComponentData;
		public override string DisplayName => "DefaultHealthComponent";

		public DefaultHealthComponent(FrostySdk.Ebx.DefaultHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

