using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaypointsWalkerEntityData))]
	public class WaypointsWalkerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WaypointsWalkerEntityData>
	{
		public new FrostySdk.Ebx.WaypointsWalkerEntityData Data => data as FrostySdk.Ebx.WaypointsWalkerEntityData;
		public override string DisplayName => "WaypointsWalker";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WaypointsWalkerEntity(FrostySdk.Ebx.WaypointsWalkerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

