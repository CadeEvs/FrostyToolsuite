using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UISphereTargetingEntityData))]
	public class UISphereTargetingEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UISphereTargetingEntityData>
	{
		public new FrostySdk.Ebx.UISphereTargetingEntityData Data => data as FrostySdk.Ebx.UISphereTargetingEntityData;
		public override string DisplayName => "UISphereTargeting";

		public UISphereTargetingEntity(FrostySdk.Ebx.UISphereTargetingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

