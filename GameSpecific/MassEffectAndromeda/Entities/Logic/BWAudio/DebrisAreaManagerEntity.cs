using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebrisAreaManagerEntityData))]
	public class DebrisAreaManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DebrisAreaManagerEntityData>
	{
		public new FrostySdk.Ebx.DebrisAreaManagerEntityData Data => data as FrostySdk.Ebx.DebrisAreaManagerEntityData;
		public override string DisplayName => "DebrisAreaManager";

		public DebrisAreaManagerEntity(FrostySdk.Ebx.DebrisAreaManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

