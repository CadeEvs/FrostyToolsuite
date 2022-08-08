using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEOffsetControllerData))]
	public class MEOffsetController : OffsetController, IEntityData<FrostySdk.Ebx.MEOffsetControllerData>
	{
		public new FrostySdk.Ebx.MEOffsetControllerData Data => data as FrostySdk.Ebx.MEOffsetControllerData;
		public override string DisplayName => "MEOffsetController";

		public MEOffsetController(FrostySdk.Ebx.MEOffsetControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

