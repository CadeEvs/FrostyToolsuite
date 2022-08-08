using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RGBToHSVEntityData))]
	public class RGBToHSVEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RGBToHSVEntityData>
	{
		public new FrostySdk.Ebx.RGBToHSVEntityData Data => data as FrostySdk.Ebx.RGBToHSVEntityData;
		public override string DisplayName => "RGBToHSV";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RGBToHSVEntity(FrostySdk.Ebx.RGBToHSVEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

