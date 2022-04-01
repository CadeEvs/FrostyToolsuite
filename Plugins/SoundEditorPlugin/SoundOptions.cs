using Frosty.Core;
using Frosty.Core.Controls.Editors;
using FrostySdk.Attributes;
using FrostySdk.IO;

namespace SoundEditorPlugin
{
    [DisplayName("Sound Options")]
    public class SoundOptions : OptionsExtension
    {
        [Category("Editor")]
        [DisplayName("Sound Volume")]
        [Description("Playback volume for the sound.")]
        [Editor(typeof(FrostySliderEditor))]
        [SliderMinMax(0.0f, 100.0f, 1.0f, 10.0f, true)]
        [EbxFieldMeta(EbxFieldType.Float32)]
        public float Volume { get; set; } = 20.0f;

        public override void Load()
        {
            Volume = Config.Get<float>("SoundVolume", 20.0f);
            //Volume = Config.Get<float>("Editor", "SoundVolume", 50);
        }

        public override void Save()
        {
            Config.Add("SoundVolume", Volume);
            Config.Save();
            //Config.Add("Editor", "SoundVolume", Volume);
        }

        public override bool Validate() => Volume >= 0.0f && Volume <= 100.0f;
    }
}
