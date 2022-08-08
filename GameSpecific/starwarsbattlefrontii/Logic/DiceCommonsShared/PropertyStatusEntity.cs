using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyStatusEntityData))]
	public class PropertyStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyStatusEntityData>
	{
		public new FrostySdk.Ebx.PropertyStatusEntityData Data => data as FrostySdk.Ebx.PropertyStatusEntityData;
		public override string DisplayName => "PropertyStatus";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PropertyStatusEntity(FrostySdk.Ebx.PropertyStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

