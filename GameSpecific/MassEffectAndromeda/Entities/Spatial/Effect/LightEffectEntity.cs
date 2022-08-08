using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LightEffectEntityData))]
	public class LightEffectEntity : ChildEffectEntity, IEntityData<FrostySdk.Ebx.LightEffectEntityData>
	{
		public new FrostySdk.Ebx.LightEffectEntityData Data => data as FrostySdk.Ebx.LightEffectEntityData;

		public LightEffectEntity(FrostySdk.Ebx.LightEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

