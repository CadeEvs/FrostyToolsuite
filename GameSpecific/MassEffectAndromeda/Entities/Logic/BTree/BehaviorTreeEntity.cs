using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BehaviorTreeEntityData))]
	public class BehaviorTreeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BehaviorTreeEntityData>
	{
		public new FrostySdk.Ebx.BehaviorTreeEntityData Data => data as FrostySdk.Ebx.BehaviorTreeEntityData;
		public override string DisplayName => "BehaviorTree";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BehaviorTreeEntity(FrostySdk.Ebx.BehaviorTreeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

