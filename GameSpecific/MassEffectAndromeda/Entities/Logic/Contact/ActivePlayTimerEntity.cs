using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ActivePlayTimerEntityData))]
	public class ActivePlayTimerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ActivePlayTimerEntityData>
	{
		public new FrostySdk.Ebx.ActivePlayTimerEntityData Data => data as FrostySdk.Ebx.ActivePlayTimerEntityData;
		public override string DisplayName => "ActivePlayTimer";

		public ActivePlayTimerEntity(FrostySdk.Ebx.ActivePlayTimerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

