using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CaptureAmbientLocatorEntityData))]
	public class CaptureAmbientLocatorEntity : AmbientLocationProxyBase, IEntityData<FrostySdk.Ebx.CaptureAmbientLocatorEntityData>
	{
		public new FrostySdk.Ebx.CaptureAmbientLocatorEntityData Data => data as FrostySdk.Ebx.CaptureAmbientLocatorEntityData;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Capture", Direction.In)
			};
		}

		public CaptureAmbientLocatorEntity(FrostySdk.Ebx.CaptureAmbientLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

