using System.Windows;
using AnimationEditorPlugin.Editors.ViewModels;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;

namespace AnimationEditorPlugin.Editors
{
    public class AnimationEditor : FrostyBaseEditor
    {
        public AssetBankViewModel ViewModel => m_viewModel;

        private readonly AssetBankViewModel m_viewModel;
        private ILogger m_logger;
        
        static AnimationEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimationEditor), new FrameworkPropertyMetadata(typeof(AnimationEditor)));
        }
        
        public AnimationEditor(ILogger inLogger)
        {
            m_logger = inLogger;
            
            m_viewModel = new AssetBankViewModel(this);
        }
    }
}