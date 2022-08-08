using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECheckPointTimerEntityData))]
	public class MECheckPointTimerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MECheckPointTimerEntityData>
	{
		public new FrostySdk.Ebx.MECheckPointTimerEntityData Data => data as FrostySdk.Ebx.MECheckPointTimerEntityData;
		public override string DisplayName => "MECheckPointTimer";

		public MECheckPointTimerEntity(FrostySdk.Ebx.MECheckPointTimerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

