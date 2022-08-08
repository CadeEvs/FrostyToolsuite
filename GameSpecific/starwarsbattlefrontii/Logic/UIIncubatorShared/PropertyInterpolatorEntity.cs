using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyInterpolatorEntityData))]
	public class PropertyInterpolatorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyInterpolatorEntityData>
	{
		public new FrostySdk.Ebx.PropertyInterpolatorEntityData Data => data as FrostySdk.Ebx.PropertyInterpolatorEntityData;
		public override string DisplayName => "PropertyInterpolator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PropertyInterpolatorEntity(FrostySdk.Ebx.PropertyInterpolatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

