using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterEntryComponentData))]
	public class CharacterEntryComponent : EntryComponent, IEntityData<FrostySdk.Ebx.CharacterEntryComponentData>
	{
		public new FrostySdk.Ebx.CharacterEntryComponentData Data => data as FrostySdk.Ebx.CharacterEntryComponentData;
		public override string DisplayName => "CharacterEntryComponent";

		public CharacterEntryComponent(FrostySdk.Ebx.CharacterEntryComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

