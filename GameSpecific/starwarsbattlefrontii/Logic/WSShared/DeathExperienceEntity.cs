using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DeathExperienceEntityData))]
	public class DeathExperienceEntity : DiceShooterDeathExperienceEntity, IEntityData<FrostySdk.Ebx.DeathExperienceEntityData>
	{
		public new FrostySdk.Ebx.DeathExperienceEntityData Data => data as FrostySdk.Ebx.DeathExperienceEntityData;
		public override string DisplayName => "DeathExperience";

		public DeathExperienceEntity(FrostySdk.Ebx.DeathExperienceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

