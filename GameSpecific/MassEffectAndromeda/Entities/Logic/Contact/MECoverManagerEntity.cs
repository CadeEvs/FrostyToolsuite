using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECoverManagerEntityData))]
	public class MECoverManagerEntity : CoverManagerEntity, IEntityData<FrostySdk.Ebx.MECoverManagerEntityData>
	{
		public new FrostySdk.Ebx.MECoverManagerEntityData Data => data as FrostySdk.Ebx.MECoverManagerEntityData;
		public override string DisplayName => "MECoverManager";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MECoverManagerEntity(FrostySdk.Ebx.MECoverManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

