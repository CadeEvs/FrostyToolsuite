using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetLevelInformationEntityData))]
	public class SetLevelInformationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetLevelInformationEntityData>
	{
		public new FrostySdk.Ebx.SetLevelInformationEntityData Data => data as FrostySdk.Ebx.SetLevelInformationEntityData;
		public override string DisplayName => "SetLevelInformation";

		public SetLevelInformationEntity(FrostySdk.Ebx.SetLevelInformationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

