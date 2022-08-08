using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PrioritizedBoolEntityData))]
	public class PrioritizedBoolEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PrioritizedBoolEntityData>
	{
		public new FrostySdk.Ebx.PrioritizedBoolEntityData Data => data as FrostySdk.Ebx.PrioritizedBoolEntityData;
		public override string DisplayName => "PrioritizedBool";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PrioritizedBoolEntity(FrostySdk.Ebx.PrioritizedBoolEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

