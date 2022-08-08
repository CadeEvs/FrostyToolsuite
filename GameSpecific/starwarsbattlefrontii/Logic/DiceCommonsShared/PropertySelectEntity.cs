using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertySelectEntityData))]
	public class PropertySelectEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertySelectEntityData>
	{
		public new FrostySdk.Ebx.PropertySelectEntityData Data => data as FrostySdk.Ebx.PropertySelectEntityData;
		public override string DisplayName => "PropertySelect";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PropertySelectEntity(FrostySdk.Ebx.PropertySelectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

