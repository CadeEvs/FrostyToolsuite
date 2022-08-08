using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AwarenessDisplayElementData))]
	public class AwarenessDisplayElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.AwarenessDisplayElementData>
	{
		public new FrostySdk.Ebx.AwarenessDisplayElementData Data => data as FrostySdk.Ebx.AwarenessDisplayElementData;
		public override string DisplayName => "AwarenessDisplayElement";

		public AwarenessDisplayElement(FrostySdk.Ebx.AwarenessDisplayElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

