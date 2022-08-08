using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PA2TargetEntityData))]
	public class PA2TargetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PA2TargetEntityData>
	{
		public new FrostySdk.Ebx.PA2TargetEntityData Data => data as FrostySdk.Ebx.PA2TargetEntityData;
		public override string DisplayName => "PA2Target";

		public PA2TargetEntity(FrostySdk.Ebx.PA2TargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

