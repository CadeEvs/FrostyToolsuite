using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SubLevelVisibleEntityData))]
	public class SubLevelVisibleEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SubLevelVisibleEntityData>
	{
		public new FrostySdk.Ebx.SubLevelVisibleEntityData Data => data as FrostySdk.Ebx.SubLevelVisibleEntityData;
		public override string DisplayName => "SubLevelVisible";

		public SubLevelVisibleEntity(FrostySdk.Ebx.SubLevelVisibleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

