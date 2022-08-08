using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocatorTransposerData))]
	public class LocatorTransposer : CinebotTransposer, IEntityData<FrostySdk.Ebx.LocatorTransposerData>
	{
		public new FrostySdk.Ebx.LocatorTransposerData Data => data as FrostySdk.Ebx.LocatorTransposerData;
		public override string DisplayName => "LocatorTransposer";

		public LocatorTransposer(FrostySdk.Ebx.LocatorTransposerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

