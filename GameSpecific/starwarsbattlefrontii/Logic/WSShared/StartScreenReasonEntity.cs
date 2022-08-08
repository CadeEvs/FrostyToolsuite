using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StartScreenReasonEntityData))]
	public class StartScreenReasonEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StartScreenReasonEntityData>
	{
		public new FrostySdk.Ebx.StartScreenReasonEntityData Data => data as FrostySdk.Ebx.StartScreenReasonEntityData;
		public override string DisplayName => "StartScreenReason";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StartScreenReasonEntity(FrostySdk.Ebx.StartScreenReasonEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

