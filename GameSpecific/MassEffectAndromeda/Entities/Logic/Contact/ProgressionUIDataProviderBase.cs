using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProgressionUIDataProviderBaseData))]
	public class ProgressionUIDataProviderBase : LogicEntity, IEntityData<FrostySdk.Ebx.ProgressionUIDataProviderBaseData>
	{
		public new FrostySdk.Ebx.ProgressionUIDataProviderBaseData Data => data as FrostySdk.Ebx.ProgressionUIDataProviderBaseData;
		public override string DisplayName => "ProgressionUIDataProviderBase";

		public ProgressionUIDataProviderBase(FrostySdk.Ebx.ProgressionUIDataProviderBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

