using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RenderToTextureMaskEntityData))]
	public class RenderToTextureMaskEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RenderToTextureMaskEntityData>
	{
		public new FrostySdk.Ebx.RenderToTextureMaskEntityData Data => data as FrostySdk.Ebx.RenderToTextureMaskEntityData;
		public override string DisplayName => "RenderToTextureMask";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Enable", Direction.In),
				new ConnectionDesc("Disable", Direction.In)
			};
		}

		public RenderToTextureMaskEntity(FrostySdk.Ebx.RenderToTextureMaskEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

