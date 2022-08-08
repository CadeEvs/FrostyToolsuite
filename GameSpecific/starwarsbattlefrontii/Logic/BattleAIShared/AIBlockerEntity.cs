using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIBlockerEntityData))]
	public class AIBlockerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AIBlockerEntityData>
	{
		public new FrostySdk.Ebx.AIBlockerEntityData Data => data as FrostySdk.Ebx.AIBlockerEntityData;
		public override string DisplayName => "AIBlocker";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AIBlockerEntity(FrostySdk.Ebx.AIBlockerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

