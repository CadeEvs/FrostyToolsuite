using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ImpulseControllerData))]
	public class ImpulseController : CinebotController, IEntityData<FrostySdk.Ebx.ImpulseControllerData>
	{
		public new FrostySdk.Ebx.ImpulseControllerData Data => data as FrostySdk.Ebx.ImpulseControllerData;
		public override string DisplayName => "ImpulseController";

		public ImpulseController(FrostySdk.Ebx.ImpulseControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

