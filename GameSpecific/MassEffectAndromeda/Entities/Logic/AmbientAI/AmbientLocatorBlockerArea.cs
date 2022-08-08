using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AmbientLocatorBlockerAreaData))]
	public class AmbientLocatorBlockerArea : LogicEntity, IEntityData<FrostySdk.Ebx.AmbientLocatorBlockerAreaData>
	{
		public new FrostySdk.Ebx.AmbientLocatorBlockerAreaData Data => data as FrostySdk.Ebx.AmbientLocatorBlockerAreaData;
		public override string DisplayName => "AmbientLocatorBlockerArea";

		public AmbientLocatorBlockerArea(FrostySdk.Ebx.AmbientLocatorBlockerAreaData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

