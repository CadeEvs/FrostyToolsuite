using System.IO;
using System.Windows;
using AnimationEditorPlugin.Editors.ViewModels;
using Frosty.Core;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;

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
            
            // initialize asset bank manager
            FrostyTaskWindow.Show("Importing File", "", (task) =>
            {
                App.AssetManager.SendManagerCommand("assetbank", "initialize", task.TaskLogger);
            });
            
            m_viewModel = new AssetBankViewModel(this);
        }
    }
}