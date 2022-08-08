using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyDefaultValueEntityData))]
	public class PropertyDefaultValueEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyDefaultValueEntityData>
	{
		public new FrostySdk.Ebx.PropertyDefaultValueEntityData Data => data as FrostySdk.Ebx.PropertyDefaultValueEntityData;
		public override string DisplayName => "PropertyDefaultValue";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PropertyDefaultValueEntity(FrostySdk.Ebx.PropertyDefaultValueEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

