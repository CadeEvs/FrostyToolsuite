using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectiveDisplayElementData))]
	public class ObjectiveDisplayElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.ObjectiveDisplayElementData>
	{
		public new FrostySdk.Ebx.ObjectiveDisplayElementData Data => data as FrostySdk.Ebx.ObjectiveDisplayElementData;
		public override string DisplayName => "ObjectiveDisplayElement";

		public ObjectiveDisplayElement(FrostySdk.Ebx.ObjectiveDisplayElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

