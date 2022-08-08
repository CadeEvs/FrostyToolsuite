using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSObjectiveStateEntityData))]
	public class WSObjectiveStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSObjectiveStateEntityData>
	{
		public new FrostySdk.Ebx.WSObjectiveStateEntityData Data => data as FrostySdk.Ebx.WSObjectiveStateEntityData;
		public override string DisplayName => "WSObjectiveState";

		public WSObjectiveStateEntity(FrostySdk.Ebx.WSObjectiveStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

