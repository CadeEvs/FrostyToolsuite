using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StressEntityData))]
	public class StressEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StressEntityData>
	{
		public new FrostySdk.Ebx.StressEntityData Data => data as FrostySdk.Ebx.StressEntityData;
		public override string DisplayName => "Stress";

		public StressEntity(FrostySdk.Ebx.StressEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

