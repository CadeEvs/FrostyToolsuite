using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SchematicsAimEntityData))]
	public class SchematicsAimEntity : AimEntityBase, IEntityData<FrostySdk.Ebx.SchematicsAimEntityData>
	{
		public new FrostySdk.Ebx.SchematicsAimEntityData Data => data as FrostySdk.Ebx.SchematicsAimEntityData;
		public override string DisplayName => "SchematicsAim";

		public SchematicsAimEntity(FrostySdk.Ebx.SchematicsAimEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

