using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WorldSpaceComposerData))]
	public class WorldSpaceComposer : CinebotComposer, IEntityData<FrostySdk.Ebx.WorldSpaceComposerData>
	{
		public new FrostySdk.Ebx.WorldSpaceComposerData Data => data as FrostySdk.Ebx.WorldSpaceComposerData;
		public override string DisplayName => "WorldSpaceComposer";

		public WorldSpaceComposer(FrostySdk.Ebx.WorldSpaceComposerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

