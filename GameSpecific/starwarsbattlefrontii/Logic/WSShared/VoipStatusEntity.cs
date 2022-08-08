using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoipStatusEntityData))]
	public class VoipStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoipStatusEntityData>
	{
		public new FrostySdk.Ebx.VoipStatusEntityData Data => data as FrostySdk.Ebx.VoipStatusEntityData;
		public override string DisplayName => "VoipStatus";

		public VoipStatusEntity(FrostySdk.Ebx.VoipStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

