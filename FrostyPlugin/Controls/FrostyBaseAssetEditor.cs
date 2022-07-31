using System.Windows;
using System.Windows.Input;
using Frosty.Core.Managers;
using FrostySdk.Interfaces;

namespace Frosty.Core.Controls
{
    public class FrostyBaseAssetEditor : FrostyAssetEditor
    {
        protected bool firstTimeLoad = true;
        protected RoutedCommand undoCommand;

        protected UndoManager undoManager;
        
        public FrostyBaseAssetEditor(ILogger inLogger)
            : base(inLogger)
        {
            undoManager = UndoManager.Create();
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += BaseAssetEditor_Loaded;
            Unloaded += BaseAssetEditor_Unloaded;

            undoCommand = new RoutedCommand();
            undoCommand.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));

            CommandBindings.Add(new CommandBinding(undoCommand, BaseAssetEditor_Undo));
        }
        
        private void BaseAssetEditor_Loaded(object sender, RoutedEventArgs e)
        {
            UndoManager.SetCurrent(undoManager);

            if (firstTimeLoad)
            {
                Initialize();
                firstTimeLoad = false;
            }

            Reload();
        }
        
        private void BaseAssetEditor_Unloaded(object sender, RoutedEventArgs e)
        {
            Unload();
        }
        
        private void BaseAssetEditor_Undo(object sender, RoutedEventArgs e)
        {
            UndoManager.Instance.Undo();
        }
        
        public override void Closed()
        {
            UndoManager.ClearCurrent();
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void Reload()
        {
        }

        protected virtual void Unload()
        {
        }
    }
}