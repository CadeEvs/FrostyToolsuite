using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NewWaveAssetObserverEntityData))]
	public class NewWaveAssetObserverEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NewWaveAssetObserverEntityData>
	{
		public new FrostySdk.Ebx.NewWaveAssetObserverEntityData Data => data as FrostySdk.Ebx.NewWaveAssetObserverEntityData;
		public override string DisplayName => "NewWaveAssetObserver";

		public NewWaveAssetObserverEntity(FrostySdk.Ebx.NewWaveAssetObserverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

