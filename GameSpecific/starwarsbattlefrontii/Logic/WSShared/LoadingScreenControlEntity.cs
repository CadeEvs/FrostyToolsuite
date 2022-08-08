using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LoadingScreenControlEntityData))]
	public class LoadingScreenControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LoadingScreenControlEntityData>
	{
		public new FrostySdk.Ebx.LoadingScreenControlEntityData Data => data as FrostySdk.Ebx.LoadingScreenControlEntityData;
		public override string DisplayName => "LoadingScreenControl";

		public LoadingScreenControlEntity(FrostySdk.Ebx.LoadingScreenControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

