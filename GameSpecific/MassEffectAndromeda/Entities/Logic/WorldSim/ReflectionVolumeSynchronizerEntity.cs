using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReflectionVolumeSynchronizerEntityData))]
	public class ReflectionVolumeSynchronizerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ReflectionVolumeSynchronizerEntityData>
	{
		public new FrostySdk.Ebx.ReflectionVolumeSynchronizerEntityData Data => data as FrostySdk.Ebx.ReflectionVolumeSynchronizerEntityData;
		public override string DisplayName => "ReflectionVolumeSynchronizer";

		public ReflectionVolumeSynchronizerEntity(FrostySdk.Ebx.ReflectionVolumeSynchronizerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

