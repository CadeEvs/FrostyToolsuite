using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicPlayerAbilityPickupEntityData))]
	public class DynamicPlayerAbilityPickupEntity : PlayerAbilitySpatialPickupEntity, IEntityData<FrostySdk.Ebx.DynamicPlayerAbilityPickupEntityData>
	{
		public new FrostySdk.Ebx.DynamicPlayerAbilityPickupEntityData Data => data as FrostySdk.Ebx.DynamicPlayerAbilityPickupEntityData;

		public DynamicPlayerAbilityPickupEntity(FrostySdk.Ebx.DynamicPlayerAbilityPickupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

