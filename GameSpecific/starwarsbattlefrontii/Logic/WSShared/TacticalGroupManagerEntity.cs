using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TacticalGroupManagerEntityData))]
	public class TacticalGroupManagerEntity : SpawnGroupManagerEntity, IEntityData<FrostySdk.Ebx.TacticalGroupManagerEntityData>
	{
		public new FrostySdk.Ebx.TacticalGroupManagerEntityData Data => data as FrostySdk.Ebx.TacticalGroupManagerEntityData;
		public override string DisplayName => "TacticalGroupManager";

		public TacticalGroupManagerEntity(FrostySdk.Ebx.TacticalGroupManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

