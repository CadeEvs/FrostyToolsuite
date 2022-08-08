using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoipControlEntityData))]
	public class VoipControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoipControlEntityData>
	{
		public new FrostySdk.Ebx.VoipControlEntityData Data => data as FrostySdk.Ebx.VoipControlEntityData;
		public override string DisplayName => "VoipControl";

		public VoipControlEntity(FrostySdk.Ebx.VoipControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

