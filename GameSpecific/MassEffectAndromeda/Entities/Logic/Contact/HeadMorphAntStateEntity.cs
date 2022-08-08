using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeadMorphAntStateEntityData))]
	public class HeadMorphAntStateEntity : HeadMorphItemEntity, IEntityData<FrostySdk.Ebx.HeadMorphAntStateEntityData>
	{
		public new FrostySdk.Ebx.HeadMorphAntStateEntityData Data => data as FrostySdk.Ebx.HeadMorphAntStateEntityData;
		public override string DisplayName => "HeadMorphAntState";

		public HeadMorphAntStateEntity(FrostySdk.Ebx.HeadMorphAntStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

