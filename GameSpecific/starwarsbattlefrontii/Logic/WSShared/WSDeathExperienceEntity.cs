using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSDeathExperienceEntityData))]
	public class WSDeathExperienceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSDeathExperienceEntityData>
	{
		public new FrostySdk.Ebx.WSDeathExperienceEntityData Data => data as FrostySdk.Ebx.WSDeathExperienceEntityData;
		public override string DisplayName => "WSDeathExperience";

		public WSDeathExperienceEntity(FrostySdk.Ebx.WSDeathExperienceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

