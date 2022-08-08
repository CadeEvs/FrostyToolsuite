using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EmitterEntityData))]
	public class EmitterEntity : EmitterChildEffectEntity, IEntityData<FrostySdk.Ebx.EmitterEntityData>
	{
		public new FrostySdk.Ebx.EmitterEntityData Data => data as FrostySdk.Ebx.EmitterEntityData;

		public EmitterEntity(FrostySdk.Ebx.EmitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

