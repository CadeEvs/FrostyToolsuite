using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AimEntityBaseData))]
	public class AimEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.AimEntityBaseData>
	{
		public new FrostySdk.Ebx.AimEntityBaseData Data => data as FrostySdk.Ebx.AimEntityBaseData;
		public override string DisplayName => "AimEntityBase";

		public AimEntityBase(FrostySdk.Ebx.AimEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

