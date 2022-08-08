using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetGalaxyObjectUIDataEntityData))]
	public class GetGalaxyObjectUIDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GetGalaxyObjectUIDataEntityData>
	{
		public new FrostySdk.Ebx.GetGalaxyObjectUIDataEntityData Data => data as FrostySdk.Ebx.GetGalaxyObjectUIDataEntityData;
		public override string DisplayName => "GetGalaxyObjectUIData";

		public GetGalaxyObjectUIDataEntity(FrostySdk.Ebx.GetGalaxyObjectUIDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

