using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetPrioritisationEntityData))]
	public class TargetPrioritisationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TargetPrioritisationEntityData>
	{
		public new FrostySdk.Ebx.TargetPrioritisationEntityData Data => data as FrostySdk.Ebx.TargetPrioritisationEntityData;
		public override string DisplayName => "TargetPrioritisation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TargetPrioritisationEntity(FrostySdk.Ebx.TargetPrioritisationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

