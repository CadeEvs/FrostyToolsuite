using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FrozenCharacterPoseComponentData))]
	public class FrozenCharacterPoseComponent : GameComponent, IEntityData<FrostySdk.Ebx.FrozenCharacterPoseComponentData>
	{
		public new FrostySdk.Ebx.FrozenCharacterPoseComponentData Data => data as FrostySdk.Ebx.FrozenCharacterPoseComponentData;
		public override string DisplayName => "FrozenCharacterPoseComponent";

		public FrozenCharacterPoseComponent(FrostySdk.Ebx.FrozenCharacterPoseComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

