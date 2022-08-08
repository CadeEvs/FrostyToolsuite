using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalPropertyEntityData))]
	public class ConditionalPropertyEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ConditionalPropertyEntityData>
	{
		public new FrostySdk.Ebx.ConditionalPropertyEntityData Data => data as FrostySdk.Ebx.ConditionalPropertyEntityData;
		public override string DisplayName => "ConditionalProperty";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ConditionalPropertyEntity(FrostySdk.Ebx.ConditionalPropertyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

