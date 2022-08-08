using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterMorphHeadComponentData))]
	public class CharacterMorphHeadComponent : DynamicMorphComponent, IEntityData<FrostySdk.Ebx.CharacterMorphHeadComponentData>
	{
		public new FrostySdk.Ebx.CharacterMorphHeadComponentData Data => data as FrostySdk.Ebx.CharacterMorphHeadComponentData;
		public override string DisplayName => "CharacterMorphHeadComponent";

		public CharacterMorphHeadComponent(FrostySdk.Ebx.CharacterMorphHeadComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

