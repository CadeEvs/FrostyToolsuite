using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GlyphTerminalData))]
	public class GlyphTerminal : GameComponent, IEntityData<FrostySdk.Ebx.GlyphTerminalData>
	{
		public new FrostySdk.Ebx.GlyphTerminalData Data => data as FrostySdk.Ebx.GlyphTerminalData;
		public override string DisplayName => "GlyphTerminal";

		public GlyphTerminal(FrostySdk.Ebx.GlyphTerminalData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

