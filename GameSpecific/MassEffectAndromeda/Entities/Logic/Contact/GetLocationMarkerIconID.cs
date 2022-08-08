using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetLocationMarkerIconIDData))]
	public class GetLocationMarkerIconID : LogicEntity, IEntityData<FrostySdk.Ebx.GetLocationMarkerIconIDData>
	{
		public new FrostySdk.Ebx.GetLocationMarkerIconIDData Data => data as FrostySdk.Ebx.GetLocationMarkerIconIDData;
		public override string DisplayName => "GetLocationMarkerIconID";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OutIconID", Direction.Out)
			};
		}

		public GetLocationMarkerIconID(FrostySdk.Ebx.GetLocationMarkerIconIDData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

