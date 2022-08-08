using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProgressVectorShapeElementData))]
	public class ProgressVectorShapeElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.ProgressVectorShapeElementData>
	{
		public new FrostySdk.Ebx.ProgressVectorShapeElementData Data => data as FrostySdk.Ebx.ProgressVectorShapeElementData;
		public override string DisplayName => "ProgressVectorShapeElement";

		public ProgressVectorShapeElement(FrostySdk.Ebx.ProgressVectorShapeElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

