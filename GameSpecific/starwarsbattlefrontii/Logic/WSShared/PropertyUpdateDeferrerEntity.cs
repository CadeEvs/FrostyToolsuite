using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyUpdateDeferrerEntityData))]
	public class PropertyUpdateDeferrerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyUpdateDeferrerEntityData>
	{
		public new FrostySdk.Ebx.PropertyUpdateDeferrerEntityData Data => data as FrostySdk.Ebx.PropertyUpdateDeferrerEntityData;
		public override string DisplayName => "PropertyUpdateDeferrer";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PropertyUpdateDeferrerEntity(FrostySdk.Ebx.PropertyUpdateDeferrerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

