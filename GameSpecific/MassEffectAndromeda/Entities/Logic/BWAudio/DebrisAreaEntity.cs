using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebrisAreaEntityData))]
	public class DebrisAreaEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DebrisAreaEntityData>
	{
		public new FrostySdk.Ebx.DebrisAreaEntityData Data => data as FrostySdk.Ebx.DebrisAreaEntityData;
		public override string DisplayName => "DebrisArea";

		public DebrisAreaEntity(FrostySdk.Ebx.DebrisAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

