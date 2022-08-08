using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardManagerEntityData))]
	public class BlackboardManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BlackboardManagerEntityData>
	{
		public new FrostySdk.Ebx.BlackboardManagerEntityData Data => data as FrostySdk.Ebx.BlackboardManagerEntityData;
		public override string DisplayName => "BlackboardManager";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardManagerEntity(FrostySdk.Ebx.BlackboardManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

