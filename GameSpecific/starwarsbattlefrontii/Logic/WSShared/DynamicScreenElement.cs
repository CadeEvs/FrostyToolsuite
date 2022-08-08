using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicScreenElementData))]
	public class DynamicScreenElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.DynamicScreenElementData>
	{
		public new FrostySdk.Ebx.DynamicScreenElementData Data => data as FrostySdk.Ebx.DynamicScreenElementData;
		public override string DisplayName => "DynamicScreenElement";

		public DynamicScreenElement(FrostySdk.Ebx.DynamicScreenElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

