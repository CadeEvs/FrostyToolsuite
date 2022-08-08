using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UAVManagerEntityData))]
	public class UAVManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UAVManagerEntityData>
	{
		public new FrostySdk.Ebx.UAVManagerEntityData Data => data as FrostySdk.Ebx.UAVManagerEntityData;
		public override string DisplayName => "UAVManager";

		public UAVManagerEntity(FrostySdk.Ebx.UAVManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

