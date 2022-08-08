using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StateNodeEntityBaseData))]
	public class StateNodeEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.StateNodeEntityBaseData>
	{
		public new FrostySdk.Ebx.StateNodeEntityBaseData Data => data as FrostySdk.Ebx.StateNodeEntityBaseData;
		public override string DisplayName => "StateNodeEntityBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StateNodeEntityBase(FrostySdk.Ebx.StateNodeEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

