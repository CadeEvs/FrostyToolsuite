using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceShooterDeathExperienceEntityData))]
	public class DiceShooterDeathExperienceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DiceShooterDeathExperienceEntityData>
	{
		public new FrostySdk.Ebx.DiceShooterDeathExperienceEntityData Data => data as FrostySdk.Ebx.DiceShooterDeathExperienceEntityData;
		public override string DisplayName => "DiceShooterDeathExperience";

		public DiceShooterDeathExperienceEntity(FrostySdk.Ebx.DiceShooterDeathExperienceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

