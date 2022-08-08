using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocatorComposerData))]
	public class LocatorComposer : CinebotComposer, IEntityData<FrostySdk.Ebx.LocatorComposerData>
	{
		public new FrostySdk.Ebx.LocatorComposerData Data => data as FrostySdk.Ebx.LocatorComposerData;
		public override string DisplayName => "LocatorComposer";

		public LocatorComposer(FrostySdk.Ebx.LocatorComposerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

