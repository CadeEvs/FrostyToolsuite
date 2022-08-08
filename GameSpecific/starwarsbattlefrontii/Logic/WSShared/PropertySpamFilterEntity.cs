using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertySpamFilterEntityData))]
	public class PropertySpamFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertySpamFilterEntityData>
	{
		public new FrostySdk.Ebx.PropertySpamFilterEntityData Data => data as FrostySdk.Ebx.PropertySpamFilterEntityData;
		public override string DisplayName => "PropertySpamFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PropertySpamFilterEntity(FrostySdk.Ebx.PropertySpamFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

