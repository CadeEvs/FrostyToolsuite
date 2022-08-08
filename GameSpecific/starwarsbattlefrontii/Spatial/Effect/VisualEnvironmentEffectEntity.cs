using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VisualEnvironmentEffectEntityData))]
	public class VisualEnvironmentEffectEntity : ChildEffectEntity, IEntityData<FrostySdk.Ebx.VisualEnvironmentEffectEntityData>
	{
		public new FrostySdk.Ebx.VisualEnvironmentEffectEntityData Data => data as FrostySdk.Ebx.VisualEnvironmentEffectEntityData;

		public VisualEnvironmentEffectEntity(FrostySdk.Ebx.VisualEnvironmentEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

