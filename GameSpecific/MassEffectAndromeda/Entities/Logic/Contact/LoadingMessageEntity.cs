using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LoadingMessageEntityData))]
	public class LoadingMessageEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LoadingMessageEntityData>
	{
		public new FrostySdk.Ebx.LoadingMessageEntityData Data => data as FrostySdk.Ebx.LoadingMessageEntityData;
		public override string DisplayName => "LoadingMessage";

		public LoadingMessageEntity(FrostySdk.Ebx.LoadingMessageEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

