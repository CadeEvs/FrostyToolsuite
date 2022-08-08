using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FirstPartyShowMessageEntityData))]
	public class FirstPartyShowMessageEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FirstPartyShowMessageEntityData>
	{
		public new FrostySdk.Ebx.FirstPartyShowMessageEntityData Data => data as FrostySdk.Ebx.FirstPartyShowMessageEntityData;
		public override string DisplayName => "FirstPartyShowMessage";

		public FirstPartyShowMessageEntity(FrostySdk.Ebx.FirstPartyShowMessageEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

