using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocationEffectEntityData))]
	public class LocationEffectEntity : ChildEffectEntity, IEntityData<FrostySdk.Ebx.LocationEffectEntityData>
	{
		public new FrostySdk.Ebx.LocationEffectEntityData Data => data as FrostySdk.Ebx.LocationEffectEntityData;

		public LocationEffectEntity(FrostySdk.Ebx.LocationEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

