using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KillCounterEntityData))]
	public class KillCounterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.KillCounterEntityData>
	{
		public new FrostySdk.Ebx.KillCounterEntityData Data => data as FrostySdk.Ebx.KillCounterEntityData;
		public override string DisplayName => "KillCounter";

		public KillCounterEntity(FrostySdk.Ebx.KillCounterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

