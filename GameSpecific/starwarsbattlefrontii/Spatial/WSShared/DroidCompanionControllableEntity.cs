using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionControllableEntityData))]
	public class DroidCompanionControllableEntity : ControllableEntity, IEntityData<FrostySdk.Ebx.DroidCompanionControllableEntityData>
	{
		public new FrostySdk.Ebx.DroidCompanionControllableEntityData Data => data as FrostySdk.Ebx.DroidCompanionControllableEntityData;

		public DroidCompanionControllableEntity(FrostySdk.Ebx.DroidCompanionControllableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

