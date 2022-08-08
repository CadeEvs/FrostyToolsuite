using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESaveMenuListenerEntityData))]
	public class MESaveMenuListenerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MESaveMenuListenerEntityData>
	{
		public new FrostySdk.Ebx.MESaveMenuListenerEntityData Data => data as FrostySdk.Ebx.MESaveMenuListenerEntityData;
		public override string DisplayName => "MESaveMenuListener";

		public MESaveMenuListenerEntity(FrostySdk.Ebx.MESaveMenuListenerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

