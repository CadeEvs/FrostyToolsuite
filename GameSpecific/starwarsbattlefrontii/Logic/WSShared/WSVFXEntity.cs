using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSVFXEntityData))]
	public class WSVFXEntity : DicePlayVFXEntity, IEntityData<FrostySdk.Ebx.WSVFXEntityData>
	{
		public new FrostySdk.Ebx.WSVFXEntityData Data => data as FrostySdk.Ebx.WSVFXEntityData;
		public override string DisplayName => "WSVFX";

		public WSVFXEntity(FrostySdk.Ebx.WSVFXEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

