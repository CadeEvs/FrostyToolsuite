using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConversationControllerEntityData))]
	public class ConversationControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ConversationControllerEntityData>
	{
		public new FrostySdk.Ebx.ConversationControllerEntityData Data => data as FrostySdk.Ebx.ConversationControllerEntityData;
		public override string DisplayName => "ConversationController";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Start", Direction.In),
				new ConnectionDesc("Update", Direction.In)
			};
		}

		public ConversationControllerEntity(FrostySdk.Ebx.ConversationControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

