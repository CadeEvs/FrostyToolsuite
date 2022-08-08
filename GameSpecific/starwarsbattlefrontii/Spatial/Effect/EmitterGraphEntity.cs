using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EmitterGraphEntityData))]
	public class EmitterGraphEntity : EmitterChildEffectEntity, IEntityData<FrostySdk.Ebx.EmitterGraphEntityData>
	{
		public new FrostySdk.Ebx.EmitterGraphEntityData Data => data as FrostySdk.Ebx.EmitterGraphEntityData;

		public EmitterGraphEntity(FrostySdk.Ebx.EmitterGraphEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

