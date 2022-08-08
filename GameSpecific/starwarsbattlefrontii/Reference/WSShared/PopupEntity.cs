using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PopupEntityData))]
	public class PopupEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.PopupEntityData>
	{
		public new FrostySdk.Ebx.PopupEntityData Data => data as FrostySdk.Ebx.PopupEntityData;

		public PopupEntity(FrostySdk.Ebx.PopupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

