using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterMeshComponentData))]
	public class CharacterMeshComponent : GameComponent, IEntityData<FrostySdk.Ebx.CharacterMeshComponentData>
	{
		public new FrostySdk.Ebx.CharacterMeshComponentData Data => data as FrostySdk.Ebx.CharacterMeshComponentData;
		public override string DisplayName => "CharacterMeshComponent";

		public CharacterMeshComponent(FrostySdk.Ebx.CharacterMeshComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

