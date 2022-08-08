using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DistanceToLocalPlayerEntityData))]
	public class DistanceToLocalPlayerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DistanceToLocalPlayerEntityData>
	{
		public new FrostySdk.Ebx.DistanceToLocalPlayerEntityData Data => data as FrostySdk.Ebx.DistanceToLocalPlayerEntityData;
		public override string DisplayName => "DistanceToLocalPlayer";

		public DistanceToLocalPlayerEntity(FrostySdk.Ebx.DistanceToLocalPlayerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

