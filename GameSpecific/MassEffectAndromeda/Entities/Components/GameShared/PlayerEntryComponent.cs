using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerEntryComponentData))]
	public class PlayerEntryComponent : EntryComponent, IEntityData<FrostySdk.Ebx.PlayerEntryComponentData>
	{
		public new FrostySdk.Ebx.PlayerEntryComponentData Data => data as FrostySdk.Ebx.PlayerEntryComponentData;
		public override string DisplayName => "PlayerEntryComponent";

		public PlayerEntryComponent(FrostySdk.Ebx.PlayerEntryComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

