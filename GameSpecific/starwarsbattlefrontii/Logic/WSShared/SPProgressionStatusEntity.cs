using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPProgressionStatusEntityData))]
	public class SPProgressionStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPProgressionStatusEntityData>
	{
		public new FrostySdk.Ebx.SPProgressionStatusEntityData Data => data as FrostySdk.Ebx.SPProgressionStatusEntityData;
		public override string DisplayName => "SPProgressionStatus";

		public SPProgressionStatusEntity(FrostySdk.Ebx.SPProgressionStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

