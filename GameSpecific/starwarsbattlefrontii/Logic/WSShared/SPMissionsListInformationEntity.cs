using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPMissionsListInformationEntityData))]
	public class SPMissionsListInformationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPMissionsListInformationEntityData>
	{
		public new FrostySdk.Ebx.SPMissionsListInformationEntityData Data => data as FrostySdk.Ebx.SPMissionsListInformationEntityData;
		public override string DisplayName => "SPMissionsListInformation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SPMissionsListInformationEntity(FrostySdk.Ebx.SPMissionsListInformationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

