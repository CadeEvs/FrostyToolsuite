using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetCurrentScreenResolutionData))]
	public class GetCurrentScreenResolution : LogicEntity, IEntityData<FrostySdk.Ebx.GetCurrentScreenResolutionData>
	{
		public new FrostySdk.Ebx.GetCurrentScreenResolutionData Data => data as FrostySdk.Ebx.GetCurrentScreenResolutionData;
		public override string DisplayName => "GetCurrentScreenResolution";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GetCurrentScreenResolution(FrostySdk.Ebx.GetCurrentScreenResolutionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

