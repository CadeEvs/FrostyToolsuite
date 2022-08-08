
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AntInputComponentData))]
	public class AntInputComponent : GameComponent, IEntityData<FrostySdk.Ebx.AntInputComponentData>
	{
		public new FrostySdk.Ebx.AntInputComponentData Data => data as FrostySdk.Ebx.AntInputComponentData;
		public override string DisplayName => "AntInputComponent";

		public AntInputComponent(FrostySdk.Ebx.AntInputComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

