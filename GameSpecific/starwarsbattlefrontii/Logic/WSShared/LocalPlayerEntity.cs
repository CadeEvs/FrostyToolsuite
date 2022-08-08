using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalPlayerEntityData))]
	public class LocalPlayerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalPlayerEntityData>
	{
		public new FrostySdk.Ebx.LocalPlayerEntityData Data => data as FrostySdk.Ebx.LocalPlayerEntityData;
		public override string DisplayName => "LocalPlayer";

		public LocalPlayerEntity(FrostySdk.Ebx.LocalPlayerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

