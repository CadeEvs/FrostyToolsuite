using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoPlayerInteractObjectiveEntityData))]
	public class AutoPlayerInteractObjectiveEntity : AutoPlayerObjectiveEntity, IEntityData<FrostySdk.Ebx.AutoPlayerInteractObjectiveEntityData>
	{
		public new FrostySdk.Ebx.AutoPlayerInteractObjectiveEntityData Data => data as FrostySdk.Ebx.AutoPlayerInteractObjectiveEntityData;
		public override string DisplayName => "AutoPlayerInteractObjective";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AutoPlayerInteractObjectiveEntity(FrostySdk.Ebx.AutoPlayerInteractObjectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

