using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EmitterChildEffectEntityData))]
	public class EmitterChildEffectEntity : ChildEffectEntity, IEntityData<FrostySdk.Ebx.EmitterChildEffectEntityData>
	{
		public new FrostySdk.Ebx.EmitterChildEffectEntityData Data => data as FrostySdk.Ebx.EmitterChildEffectEntityData;

		public EmitterChildEffectEntity(FrostySdk.Ebx.EmitterChildEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

