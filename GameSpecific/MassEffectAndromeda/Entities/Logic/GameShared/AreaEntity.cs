using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AreaEntityData))]
	public class AreaEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AreaEntityData>
	{
		public new FrostySdk.Ebx.AreaEntityData Data => data as FrostySdk.Ebx.AreaEntityData;
		public override string DisplayName => "Area";

		public AreaEntity(FrostySdk.Ebx.AreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

