using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScannerComponentData))]
	public class ScannerComponent : GameComponent, IEntityData<FrostySdk.Ebx.ScannerComponentData>
	{
		public new FrostySdk.Ebx.ScannerComponentData Data => data as FrostySdk.Ebx.ScannerComponentData;
		public override string DisplayName => "ScannerComponent";

		public ScannerComponent(FrostySdk.Ebx.ScannerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

