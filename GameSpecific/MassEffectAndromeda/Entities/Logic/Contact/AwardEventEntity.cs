using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AwardEventEntityData))]
	public class AwardEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AwardEventEntityData>
	{
		public new FrostySdk.Ebx.AwardEventEntityData Data => data as FrostySdk.Ebx.AwardEventEntityData;
		public override string DisplayName => "AwardEvent";

		public AwardEventEntity(FrostySdk.Ebx.AwardEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

