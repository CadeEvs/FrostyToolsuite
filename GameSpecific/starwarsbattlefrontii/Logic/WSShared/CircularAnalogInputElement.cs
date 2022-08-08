using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CircularAnalogInputElementData))]
	public class CircularAnalogInputElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.CircularAnalogInputElementData>
	{
		public new FrostySdk.Ebx.CircularAnalogInputElementData Data => data as FrostySdk.Ebx.CircularAnalogInputElementData;
		public override string DisplayName => "CircularAnalogInputElement";

		public CircularAnalogInputElement(FrostySdk.Ebx.CircularAnalogInputElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

