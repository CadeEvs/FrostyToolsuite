using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NestedConditionalPropertyEntityData))]
	public class NestedConditionalPropertyEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NestedConditionalPropertyEntityData>
	{
		public new FrostySdk.Ebx.NestedConditionalPropertyEntityData Data => data as FrostySdk.Ebx.NestedConditionalPropertyEntityData;
		public override string DisplayName => "NestedConditionalProperty";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public NestedConditionalPropertyEntity(FrostySdk.Ebx.NestedConditionalPropertyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

