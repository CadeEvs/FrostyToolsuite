using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatSwitchEventEntityData))]
	public class FloatSwitchEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FloatSwitchEventEntityData>
	{
		public new FrostySdk.Ebx.FloatSwitchEventEntityData Data => data as FrostySdk.Ebx.FloatSwitchEventEntityData;
		public override string DisplayName => "FloatSwitchEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FloatSwitchEventEntity(FrostySdk.Ebx.FloatSwitchEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

