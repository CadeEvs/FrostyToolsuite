using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticPlayerAbilityPickupEntityData))]
	public class StaticPlayerAbilityPickupEntity : PlayerAbilitySpatialPickupEntity, IEntityData<FrostySdk.Ebx.StaticPlayerAbilityPickupEntityData>
	{
		public new FrostySdk.Ebx.StaticPlayerAbilityPickupEntityData Data => data as FrostySdk.Ebx.StaticPlayerAbilityPickupEntityData;

		public StaticPlayerAbilityPickupEntity(FrostySdk.Ebx.StaticPlayerAbilityPickupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

