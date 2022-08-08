using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetDestinationUIDataEntityData))]
	public class GetDestinationUIDataEntity : GetGalaxyObjectUIDataEntity, IEntityData<FrostySdk.Ebx.GetDestinationUIDataEntityData>
	{
		public new FrostySdk.Ebx.GetDestinationUIDataEntityData Data => data as FrostySdk.Ebx.GetDestinationUIDataEntityData;
		public override string DisplayName => "GetDestinationUIData";

		public GetDestinationUIDataEntity(FrostySdk.Ebx.GetDestinationUIDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

