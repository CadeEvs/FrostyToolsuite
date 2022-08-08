using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RumbleEntityData))]
	public class RumbleEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RumbleEntityData>
	{
		public new FrostySdk.Ebx.RumbleEntityData Data => data as FrostySdk.Ebx.RumbleEntityData;
		public override string DisplayName => "Rumble";

		public RumbleEntity(FrostySdk.Ebx.RumbleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

