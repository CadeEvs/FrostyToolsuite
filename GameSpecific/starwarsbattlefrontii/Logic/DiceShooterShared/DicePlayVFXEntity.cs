using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DicePlayVFXEntityData))]
	public class DicePlayVFXEntity : DiceVFXEntityBase, IEntityData<FrostySdk.Ebx.DicePlayVFXEntityData>
	{
		public new FrostySdk.Ebx.DicePlayVFXEntityData Data => data as FrostySdk.Ebx.DicePlayVFXEntityData;
		public override string DisplayName => "DicePlayVFX";

		public DicePlayVFXEntity(FrostySdk.Ebx.DicePlayVFXEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

