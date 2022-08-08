using LevelEditorPlugin.Managers;
using System.Collections.Generic;
using System.IO;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundEntityData))]
	public class SoundEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundEntityData>
	{
		public new FrostySdk.Ebx.SoundEntityData Data => data as FrostySdk.Ebx.SoundEntityData;
		public override string DisplayName => "Sound";
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Enable", Direction.In),
                new ConnectionDesc("Start", Direction.In),
                new ConnectionDesc("Stop", Direction.In),
                new ConnectionDesc("Buffer", Direction.In),
                new ConnectionDesc("Buffered", Direction.Out)
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Transform", Direction.In),
                new ConnectionDesc("MasterAmplitude", Direction.In)
            };
        }
        public override IEnumerable<string> HeaderRows
        {
            get
            {
                List<string> outHeaderRows = new List<string>();
                if (soundAsset != null)
                {
                    outHeaderRows.Add(Path.GetFileName(soundAsset.Name));
                }
                return outHeaderRows;
            }
        }

        private Assets.SoundAsset soundAsset;

        public SoundEntity(FrostySdk.Ebx.SoundEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            soundAsset = LoadedAssetManager.Instance.LoadAsset<Assets.SoundAsset>(this, Data.Sound);
		}

        public override void Destroy()
        {
            LoadedAssetManager.Instance.UnloadAsset(soundAsset);
        }
    }
}

