using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpringControllerData))]
	public class SpringController : ModifierController, IEntityData<FrostySdk.Ebx.SpringControllerData>
	{
		public new FrostySdk.Ebx.SpringControllerData Data => data as FrostySdk.Ebx.SpringControllerData;
		public override string DisplayName => "SpringController";

		public SpringController(FrostySdk.Ebx.SpringControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

