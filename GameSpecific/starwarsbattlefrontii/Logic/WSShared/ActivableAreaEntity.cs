using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ActivableAreaEntityData))]
	public class ActivableAreaEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ActivableAreaEntityData>
	{
		public new FrostySdk.Ebx.ActivableAreaEntityData Data => data as FrostySdk.Ebx.ActivableAreaEntityData;
		public override string DisplayName => "ActivableArea";

		public ActivableAreaEntity(FrostySdk.Ebx.ActivableAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

