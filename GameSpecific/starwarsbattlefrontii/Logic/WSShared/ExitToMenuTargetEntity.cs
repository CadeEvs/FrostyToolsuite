using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExitToMenuTargetEntityData))]
	public class ExitToMenuTargetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ExitToMenuTargetEntityData>
	{
		public new FrostySdk.Ebx.ExitToMenuTargetEntityData Data => data as FrostySdk.Ebx.ExitToMenuTargetEntityData;
		public override string DisplayName => "ExitToMenuTarget";

		public ExitToMenuTargetEntity(FrostySdk.Ebx.ExitToMenuTargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

