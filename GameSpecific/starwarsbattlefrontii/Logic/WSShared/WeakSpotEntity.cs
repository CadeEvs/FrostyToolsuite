using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeakSpotEntityData))]
	public class WeakSpotEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WeakSpotEntityData>
	{
		public new FrostySdk.Ebx.WeakSpotEntityData Data => data as FrostySdk.Ebx.WeakSpotEntityData;
		public override string DisplayName => "WeakSpot";

		public WeakSpotEntity(FrostySdk.Ebx.WeakSpotEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

