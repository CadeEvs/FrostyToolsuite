using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GroupSizeCheckEntityData))]
	public class GroupSizeCheckEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GroupSizeCheckEntityData>
	{
		public new FrostySdk.Ebx.GroupSizeCheckEntityData Data => data as FrostySdk.Ebx.GroupSizeCheckEntityData;
		public override string DisplayName => "GroupSizeCheck";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GroupSizeCheckEntity(FrostySdk.Ebx.GroupSizeCheckEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

