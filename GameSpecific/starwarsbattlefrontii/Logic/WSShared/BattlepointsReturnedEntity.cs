using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BattlepointsReturnedEntityData))]
	public class BattlepointsReturnedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BattlepointsReturnedEntityData>
	{
		public new FrostySdk.Ebx.BattlepointsReturnedEntityData Data => data as FrostySdk.Ebx.BattlepointsReturnedEntityData;
		public override string DisplayName => "BattlepointsReturned";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BattlepointsReturnedEntity(FrostySdk.Ebx.BattlepointsReturnedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

