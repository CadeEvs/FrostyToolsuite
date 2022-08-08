using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EntryComponentData))]
	public class EntryComponent : GameComponent, IEntityData<FrostySdk.Ebx.EntryComponentData>
	{
		public new FrostySdk.Ebx.EntryComponentData Data => data as FrostySdk.Ebx.EntryComponentData;
		public override string DisplayName => "EntryComponent";

		public EntryComponent(FrostySdk.Ebx.EntryComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

