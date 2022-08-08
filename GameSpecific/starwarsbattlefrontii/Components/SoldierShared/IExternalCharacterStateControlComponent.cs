
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IExternalCharacterStateControlComponentData))]
	public class IExternalCharacterStateControlComponent : GameComponent, IEntityData<FrostySdk.Ebx.IExternalCharacterStateControlComponentData>
	{
		public new FrostySdk.Ebx.IExternalCharacterStateControlComponentData Data => data as FrostySdk.Ebx.IExternalCharacterStateControlComponentData;
		public override string DisplayName => "IExternalCharacterStateControlComponent";

		public IExternalCharacterStateControlComponent(FrostySdk.Ebx.IExternalCharacterStateControlComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

