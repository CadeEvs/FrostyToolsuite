using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ZoneStreamerVistaEntityData))]
	public class ZoneStreamerVistaEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ZoneStreamerVistaEntityData>
	{
		public new FrostySdk.Ebx.ZoneStreamerVistaEntityData Data => data as FrostySdk.Ebx.ZoneStreamerVistaEntityData;
		public override string DisplayName => "ZoneStreamerVista";

		public ZoneStreamerVistaEntity(FrostySdk.Ebx.ZoneStreamerVistaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

