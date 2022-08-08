using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ZoneGeometryEntityData))]
	public class ZoneGeometryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ZoneGeometryEntityData>
	{
		public new FrostySdk.Ebx.ZoneGeometryEntityData Data => data as FrostySdk.Ebx.ZoneGeometryEntityData;
		public override string DisplayName => "ZoneGeometry";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Links
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OBBData", Direction.Out)
			};
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Dimensions", Direction.In)
			};
        }

        public ZoneGeometryEntity(FrostySdk.Ebx.ZoneGeometryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

