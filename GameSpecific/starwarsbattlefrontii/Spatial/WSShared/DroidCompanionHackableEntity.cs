using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionHackableEntityData))]
	public class DroidCompanionHackableEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.DroidCompanionHackableEntityData>
	{
		public new FrostySdk.Ebx.DroidCompanionHackableEntityData Data => data as FrostySdk.Ebx.DroidCompanionHackableEntityData;

		public DroidCompanionHackableEntity(FrostySdk.Ebx.DroidCompanionHackableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

