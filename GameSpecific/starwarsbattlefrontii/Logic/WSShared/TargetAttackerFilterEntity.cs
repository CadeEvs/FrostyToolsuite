using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetAttackerFilterEntityData))]
	public class TargetAttackerFilterEntity : TargetFilterEntity, IEntityData<FrostySdk.Ebx.TargetAttackerFilterEntityData>
	{
		public new FrostySdk.Ebx.TargetAttackerFilterEntityData Data => data as FrostySdk.Ebx.TargetAttackerFilterEntityData;
		public override string DisplayName => "TargetAttackerFilter";

		public TargetAttackerFilterEntity(FrostySdk.Ebx.TargetAttackerFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

