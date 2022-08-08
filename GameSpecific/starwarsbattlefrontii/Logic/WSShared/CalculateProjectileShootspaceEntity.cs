using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CalculateProjectileShootspaceEntityData))]
	public class CalculateProjectileShootspaceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CalculateProjectileShootspaceEntityData>
	{
		public new FrostySdk.Ebx.CalculateProjectileShootspaceEntityData Data => data as FrostySdk.Ebx.CalculateProjectileShootspaceEntityData;
		public override string DisplayName => "CalculateProjectileShootspace";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CalculateProjectileShootspaceEntity(FrostySdk.Ebx.CalculateProjectileShootspaceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

