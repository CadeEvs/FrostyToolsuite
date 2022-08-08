using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WorldSpaceTransposerData))]
	public class WorldSpaceTransposer : CinebotTransposer, IEntityData<FrostySdk.Ebx.WorldSpaceTransposerData>
	{
		public new FrostySdk.Ebx.WorldSpaceTransposerData Data => data as FrostySdk.Ebx.WorldSpaceTransposerData;
		public override string DisplayName => "WorldSpaceTransposer";

		public WorldSpaceTransposer(FrostySdk.Ebx.WorldSpaceTransposerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

