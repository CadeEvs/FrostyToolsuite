using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EffectControllerData))]
	public class EffectController : CinebotController, IEntityData<FrostySdk.Ebx.EffectControllerData>
	{
		public new FrostySdk.Ebx.EffectControllerData Data => data as FrostySdk.Ebx.EffectControllerData;
		public override string DisplayName => "EffectController";

		public EffectController(FrostySdk.Ebx.EffectControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

