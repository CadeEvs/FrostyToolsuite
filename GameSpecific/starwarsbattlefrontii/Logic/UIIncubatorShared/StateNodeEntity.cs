using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StateNodeEntityData))]
	public class StateNodeEntity : StateNodeEntityBase, IEntityData<FrostySdk.Ebx.StateNodeEntityData>
	{
		public new FrostySdk.Ebx.StateNodeEntityData Data => data as FrostySdk.Ebx.StateNodeEntityData;
		public override string DisplayName => "StateNode";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StateNodeEntity(FrostySdk.Ebx.StateNodeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

