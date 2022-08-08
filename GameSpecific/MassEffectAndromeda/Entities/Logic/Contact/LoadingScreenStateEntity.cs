using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LoadingScreenStateEntityData))]
	public class LoadingScreenStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LoadingScreenStateEntityData>
	{
		public new FrostySdk.Ebx.LoadingScreenStateEntityData Data => data as FrostySdk.Ebx.LoadingScreenStateEntityData;
		public override string DisplayName => "LoadingScreenState";

		public LoadingScreenStateEntity(FrostySdk.Ebx.LoadingScreenStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

