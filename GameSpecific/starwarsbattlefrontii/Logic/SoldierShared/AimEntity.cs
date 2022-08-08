using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AimEntityData))]
	public class AimEntity : AimEntityBase, IEntityData<FrostySdk.Ebx.AimEntityData>
	{
		public new FrostySdk.Ebx.AimEntityData Data => data as FrostySdk.Ebx.AimEntityData;
		public override string DisplayName => "Aim";

		public AimEntity(FrostySdk.Ebx.AimEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

