using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AttackPointEntityData))]
	public class AttackPointEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.AttackPointEntityData>
	{
		public new FrostySdk.Ebx.AttackPointEntityData Data => data as FrostySdk.Ebx.AttackPointEntityData;

		public AttackPointEntity(FrostySdk.Ebx.AttackPointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

