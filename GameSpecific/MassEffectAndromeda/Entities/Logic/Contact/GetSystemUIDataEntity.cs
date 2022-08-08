using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetSystemUIDataEntityData))]
	public class GetSystemUIDataEntity : GetGalaxyObjectUIDataEntity, IEntityData<FrostySdk.Ebx.GetSystemUIDataEntityData>
	{
		public new FrostySdk.Ebx.GetSystemUIDataEntityData Data => data as FrostySdk.Ebx.GetSystemUIDataEntityData;
		public override string DisplayName => "GetSystemUIData";

		public GetSystemUIDataEntity(FrostySdk.Ebx.GetSystemUIDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

