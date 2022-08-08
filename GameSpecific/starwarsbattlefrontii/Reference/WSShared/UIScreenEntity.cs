using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIScreenEntityData))]
	public class UIScreenEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.UIScreenEntityData>
	{
		public new FrostySdk.Ebx.UIScreenEntityData Data => data as FrostySdk.Ebx.UIScreenEntityData;

		public UIScreenEntity(FrostySdk.Ebx.UIScreenEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

