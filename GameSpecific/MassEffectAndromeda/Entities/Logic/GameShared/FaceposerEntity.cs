using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FaceposerEntityData))]
	public class FaceposerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FaceposerEntityData>
	{
		public new FrostySdk.Ebx.FaceposerEntityData Data => data as FrostySdk.Ebx.FaceposerEntityData;
		public override string DisplayName => "Faceposer";

		public FaceposerEntity(FrostySdk.Ebx.FaceposerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

