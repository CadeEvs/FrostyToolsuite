using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GlyphObjectData))]
	public class GlyphObject : GameComponent, IEntityData<FrostySdk.Ebx.GlyphObjectData>
	{
		public new FrostySdk.Ebx.GlyphObjectData Data => data as FrostySdk.Ebx.GlyphObjectData;
		public override string DisplayName => "Glyph";

		public GlyphObject(FrostySdk.Ebx.GlyphObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

