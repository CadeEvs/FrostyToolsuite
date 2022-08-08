using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EyeControllerEntityData))]
	public class EyeControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EyeControllerEntityData>
	{
		public new FrostySdk.Ebx.EyeControllerEntityData Data => data as FrostySdk.Ebx.EyeControllerEntityData;
		public override string DisplayName => "EyeController";

		public EyeControllerEntity(FrostySdk.Ebx.EyeControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

