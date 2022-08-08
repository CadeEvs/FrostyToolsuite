using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AimingScaleDataProviderEntityData))]
	public class AimingScaleDataProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AimingScaleDataProviderEntityData>
	{
		public new FrostySdk.Ebx.AimingScaleDataProviderEntityData Data => data as FrostySdk.Ebx.AimingScaleDataProviderEntityData;
		public override string DisplayName => "AimingScaleDataProvider";

		public AimingScaleDataProviderEntity(FrostySdk.Ebx.AimingScaleDataProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

