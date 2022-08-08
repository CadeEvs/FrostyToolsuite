using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetBehaviorTreeEntityData))]
	public class SetBehaviorTreeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetBehaviorTreeEntityData>
	{
		public new FrostySdk.Ebx.SetBehaviorTreeEntityData Data => data as FrostySdk.Ebx.SetBehaviorTreeEntityData;
		public override string DisplayName => "SetBehaviorTree";

		public SetBehaviorTreeEntity(FrostySdk.Ebx.SetBehaviorTreeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

