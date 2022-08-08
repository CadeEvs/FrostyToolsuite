using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AggregateRigEntityData))]
	public class AggregateRigEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AggregateRigEntityData>
	{
		public new FrostySdk.Ebx.AggregateRigEntityData Data => data as FrostySdk.Ebx.AggregateRigEntityData;
		public override string DisplayName => "AggregateRig";

		public AggregateRigEntity(FrostySdk.Ebx.AggregateRigEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

