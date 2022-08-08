using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalStringEntityData))]
	public class ConditionalStringEntity : ConditionalStateEntity, IEntityData<FrostySdk.Ebx.ConditionalStringEntityData>
	{
		public new FrostySdk.Ebx.ConditionalStringEntityData Data => data as FrostySdk.Ebx.ConditionalStringEntityData;
		public override string DisplayName => "ConditionalString";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ConditionalStringEntity(FrostySdk.Ebx.ConditionalStringEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

