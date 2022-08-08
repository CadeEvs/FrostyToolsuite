using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TorusTransposerData))]
	public class TorusTransposer : CinebotTransposer, IEntityData<FrostySdk.Ebx.TorusTransposerData>
	{
		public new FrostySdk.Ebx.TorusTransposerData Data => data as FrostySdk.Ebx.TorusTransposerData;
		public override string DisplayName => "TorusTransposer";

		public TorusTransposer(FrostySdk.Ebx.TorusTransposerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

