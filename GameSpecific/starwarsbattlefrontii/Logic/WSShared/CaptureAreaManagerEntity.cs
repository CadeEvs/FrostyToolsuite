using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CaptureAreaManagerEntityData))]
	public class CaptureAreaManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CaptureAreaManagerEntityData>
	{
		public new FrostySdk.Ebx.CaptureAreaManagerEntityData Data => data as FrostySdk.Ebx.CaptureAreaManagerEntityData;
		public override string DisplayName => "CaptureAreaManager";

		public CaptureAreaManagerEntity(FrostySdk.Ebx.CaptureAreaManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

