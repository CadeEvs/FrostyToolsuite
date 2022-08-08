using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StressManagerEntityData))]
	public class StressManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StressManagerEntityData>
	{
		public new FrostySdk.Ebx.StressManagerEntityData Data => data as FrostySdk.Ebx.StressManagerEntityData;
		public override string DisplayName => "StressManager";

		public StressManagerEntity(FrostySdk.Ebx.StressManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

