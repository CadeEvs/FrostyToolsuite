using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AffectorApplyEntityData))]
	public class AffectorApplyEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AffectorApplyEntityData>
	{
		public new FrostySdk.Ebx.AffectorApplyEntityData Data => data as FrostySdk.Ebx.AffectorApplyEntityData;
		public override string DisplayName => "AffectorApply";

		public AffectorApplyEntity(FrostySdk.Ebx.AffectorApplyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

