using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScreenResolutionEntityData))]
	public class ScreenResolutionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScreenResolutionEntityData>
	{
		public new FrostySdk.Ebx.ScreenResolutionEntityData Data => data as FrostySdk.Ebx.ScreenResolutionEntityData;
		public override string DisplayName => "ScreenResolution";

		public ScreenResolutionEntity(FrostySdk.Ebx.ScreenResolutionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

