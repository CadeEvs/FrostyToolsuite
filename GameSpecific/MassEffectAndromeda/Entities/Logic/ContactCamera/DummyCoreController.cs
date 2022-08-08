using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DummyCoreControllerData))]
	public class DummyCoreController : ExtendedController, IEntityData<FrostySdk.Ebx.DummyCoreControllerData>
	{
		public new FrostySdk.Ebx.DummyCoreControllerData Data => data as FrostySdk.Ebx.DummyCoreControllerData;
		public override string DisplayName => "DummyCoreController";

		public DummyCoreController(FrostySdk.Ebx.DummyCoreControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

