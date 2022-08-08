using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LCUEntityData))]
	public class LCUEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LCUEntityData>
	{
		public new FrostySdk.Ebx.LCUEntityData Data => data as FrostySdk.Ebx.LCUEntityData;
		public override string DisplayName => "LCU";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LCUEntity(FrostySdk.Ebx.LCUEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

