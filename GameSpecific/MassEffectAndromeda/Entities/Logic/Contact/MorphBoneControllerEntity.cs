using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MorphBoneControllerEntityData))]
	public class MorphBoneControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MorphBoneControllerEntityData>
	{
		public new FrostySdk.Ebx.MorphBoneControllerEntityData Data => data as FrostySdk.Ebx.MorphBoneControllerEntityData;
		public override string DisplayName => "MorphBoneController";

		public MorphBoneControllerEntity(FrostySdk.Ebx.MorphBoneControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

