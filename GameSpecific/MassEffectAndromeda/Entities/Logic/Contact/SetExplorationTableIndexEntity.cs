using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetExplorationTableIndexEntityData))]
	public class SetExplorationTableIndexEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetExplorationTableIndexEntityData>
	{
		public new FrostySdk.Ebx.SetExplorationTableIndexEntityData Data => data as FrostySdk.Ebx.SetExplorationTableIndexEntityData;
		public override string DisplayName => "SetExplorationTableIndex";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SetExplorationTableIndexEntity(FrostySdk.Ebx.SetExplorationTableIndexEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

