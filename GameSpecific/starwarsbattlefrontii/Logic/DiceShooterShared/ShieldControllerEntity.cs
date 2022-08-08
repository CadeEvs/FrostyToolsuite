using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShieldControllerEntityData))]
	public class ShieldControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ShieldControllerEntityData>
	{
		public new FrostySdk.Ebx.ShieldControllerEntityData Data => data as FrostySdk.Ebx.ShieldControllerEntityData;
		public override string DisplayName => "ShieldController";

		public ShieldControllerEntity(FrostySdk.Ebx.ShieldControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

