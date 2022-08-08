using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PrimaryUserActionsEntityData))]
	public class PrimaryUserActionsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PrimaryUserActionsEntityData>
	{
		public new FrostySdk.Ebx.PrimaryUserActionsEntityData Data => data as FrostySdk.Ebx.PrimaryUserActionsEntityData;
		public override string DisplayName => "PrimaryUserActions";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PrimaryUserActionsEntity(FrostySdk.Ebx.PrimaryUserActionsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

