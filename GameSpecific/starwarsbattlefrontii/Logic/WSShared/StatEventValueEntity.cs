using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StatEventValueEntityData))]
	public class StatEventValueEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StatEventValueEntityData>
	{
		public new FrostySdk.Ebx.StatEventValueEntityData Data => data as FrostySdk.Ebx.StatEventValueEntityData;
		public override string DisplayName => "StatEventValue";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StatEventValueEntity(FrostySdk.Ebx.StatEventValueEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

