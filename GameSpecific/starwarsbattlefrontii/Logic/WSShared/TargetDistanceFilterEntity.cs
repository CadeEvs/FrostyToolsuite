using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetDistanceFilterEntityData))]
	public class TargetDistanceFilterEntity : TargetFilterEntity, IEntityData<FrostySdk.Ebx.TargetDistanceFilterEntityData>
	{
		public new FrostySdk.Ebx.TargetDistanceFilterEntityData Data => data as FrostySdk.Ebx.TargetDistanceFilterEntityData;
		public override string DisplayName => "TargetDistanceFilter";

		public TargetDistanceFilterEntity(FrostySdk.Ebx.TargetDistanceFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

