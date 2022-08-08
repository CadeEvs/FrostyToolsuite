using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DFSimTeamSettingsEntityData))]
	public class DFSimTeamSettingsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DFSimTeamSettingsEntityData>
	{
		public new FrostySdk.Ebx.DFSimTeamSettingsEntityData Data => data as FrostySdk.Ebx.DFSimTeamSettingsEntityData;
		public override string DisplayName => "DFSimTeamSettings";

		public DFSimTeamSettingsEntity(FrostySdk.Ebx.DFSimTeamSettingsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

