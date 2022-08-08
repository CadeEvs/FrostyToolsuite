using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebrisAreaQueryEntityData))]
	public class DebrisAreaQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DebrisAreaQueryEntityData>
	{
		public new FrostySdk.Ebx.DebrisAreaQueryEntityData Data => data as FrostySdk.Ebx.DebrisAreaQueryEntityData;
		public override string DisplayName => "DebrisAreaQuery";

		public DebrisAreaQueryEntity(FrostySdk.Ebx.DebrisAreaQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

