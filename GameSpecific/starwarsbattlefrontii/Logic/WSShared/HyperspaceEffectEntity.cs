using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HyperspaceEffectEntityData))]
	public class HyperspaceEffectEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HyperspaceEffectEntityData>
	{
		public new FrostySdk.Ebx.HyperspaceEffectEntityData Data => data as FrostySdk.Ebx.HyperspaceEffectEntityData;
		public override string DisplayName => "HyperspaceEffect";

		public HyperspaceEffectEntity(FrostySdk.Ebx.HyperspaceEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

