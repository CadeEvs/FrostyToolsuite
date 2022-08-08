using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIFirePatternEntityData))]
	public class AIFirePatternEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIFirePatternEntityData>
	{
		public new FrostySdk.Ebx.AIFirePatternEntityData Data => data as FrostySdk.Ebx.AIFirePatternEntityData;
		public override string DisplayName => "AIFirePattern";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AIFirePatternEntity(FrostySdk.Ebx.AIFirePatternEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

