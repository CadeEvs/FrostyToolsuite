using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnalogInputElementData))]
	public class AnalogInputElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.AnalogInputElementData>
	{
		public new FrostySdk.Ebx.AnalogInputElementData Data => data as FrostySdk.Ebx.AnalogInputElementData;
		public override string DisplayName => "AnalogInputElement";

		public AnalogInputElement(FrostySdk.Ebx.AnalogInputElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

