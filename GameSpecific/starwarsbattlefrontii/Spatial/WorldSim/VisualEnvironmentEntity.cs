using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VisualEnvironmentEntityData))]
	public class VisualEnvironmentEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.VisualEnvironmentEntityData>
	{
		public new FrostySdk.Ebx.VisualEnvironmentEntityData Data => data as FrostySdk.Ebx.VisualEnvironmentEntityData;

		public VisualEnvironmentEntity(FrostySdk.Ebx.VisualEnvironmentEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

