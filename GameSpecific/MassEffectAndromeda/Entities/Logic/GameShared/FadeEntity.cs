using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FadeEntityData))]
	public class FadeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FadeEntityData>
	{
		public new FrostySdk.Ebx.FadeEntityData Data => data as FrostySdk.Ebx.FadeEntityData;
		public override string DisplayName => "Fade";
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("FadeIn", Direction.In),
                new ConnectionDesc("FadeOut", Direction.In)
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get
            {
                List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
                if (Data.FadeScreen) outProperties.Add(new ConnectionDesc("ScreenFadeValue", Direction.Out));
                if (Data.FadeUI) outProperties.Add(new ConnectionDesc("UiFadeValue", Direction.Out));
                if (Data.FadeAudio) outProperties.Add(new ConnectionDesc("AudioFadeValue", Direction.Out));
                if (Data.FadeMovie) outProperties.Add(new ConnectionDesc("MovieFadeValue", Direction.Out));
                if (Data.FadeRumble) outProperties.Add(new ConnectionDesc("RumbleFadeValue", Direction.Out));
                return outProperties;
            }
        }

        public FadeEntity(FrostySdk.Ebx.FadeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

