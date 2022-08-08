using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetStarSystemPathEntityData))]
	public class GetStarSystemPathEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GetStarSystemPathEntityData>
	{
		public new FrostySdk.Ebx.GetStarSystemPathEntityData Data => data as FrostySdk.Ebx.GetStarSystemPathEntityData;
		public override string DisplayName => "GetStarSystemPath";

		public GetStarSystemPathEntity(FrostySdk.Ebx.GetStarSystemPathEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

