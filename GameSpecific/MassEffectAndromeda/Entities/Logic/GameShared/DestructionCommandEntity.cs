using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DestructionCommandEntityData))]
	public class DestructionCommandEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DestructionCommandEntityData>
	{
		public new FrostySdk.Ebx.DestructionCommandEntityData Data => data as FrostySdk.Ebx.DestructionCommandEntityData;
		public override string DisplayName => "DestructionCommand";

		public DestructionCommandEntity(FrostySdk.Ebx.DestructionCommandEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

