using System;
using System.Collections.Generic;

namespace Frosty.Core.Managers
{
    public interface IUndoUnit
    {
        string Text { get; }
        void Do();
        void Undo();
    }

    public class GenericUndoUnit : IUndoUnit
    {
        public string Text { get; private set; }

        private Action<object> doAction;
        private Action<object> undoAction;

        public GenericUndoUnit(string text, Action<object> inDoAction, Action<object> inUndoAction)
        {
            Text = text;
            doAction = inDoAction;
            undoAction = inUndoAction;
        }

        public void Do()
        {
            doAction.Invoke(null);
        }

        public void Undo()
        {
            undoAction.Invoke(null);
        }
    }

    public class UndoContainer : IUndoUnit
    {
        public string Text { get; private set; }

        public bool HasItems => children.Count > 0;
        private List<IUndoUnit> children = new List<IUndoUnit>();

        public UndoContainer(string text)
        {
            Text = text;
        }

        public void Add(IUndoUnit undoUnit)
        {
            children.Add(undoUnit);
        }

        public void Do()
        {
            foreach (var undoUnit in children)
            {
                undoUnit.Do();
            }
        }

        public void Undo()
        {
            for (int i = children.Count - 1; i >= 0; i--)
            {
                children[i].Undo();
            }
        }
    }

    public class UndoManager
    {
        #region -- Singleton --

        public static UndoManager Instance { get; private set; } = new UndoManager();
        private UndoManager() { }

        #endregion

        public IUndoUnit PendingUndoUnit
        {
            get => pendingUndoUnit;
            set
            {
                if (pendingUndoUnit != value)
                {
                    pendingUndoUnit = value;
                }
            }
        }
        public bool IsUndoing
        {
            get => isUndoing;
            set
            {
                if (isUndoing != value)
                {
                    isUndoing = value;
                }
            }
        }

        private IUndoUnit pendingUndoUnit;
        private Stack<IUndoUnit> undoStack = new Stack<IUndoUnit>();
        private Stack<IUndoUnit> redoStack = new Stack<IUndoUnit>();
        private bool isUndoing;

        public void CommitUndo(IUndoUnit undoUnit)
        {
            undoStack.Push(undoUnit);
            redoStack.Clear();

            undoUnit.Do();
            if (undoUnit == pendingUndoUnit)
            {
                pendingUndoUnit = null;
            }
        }

        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                isUndoing = true;

                IUndoUnit undoUnit = undoStack.Pop();
                redoStack.Push(undoUnit);

                App.NotificationManager.Show($"Undo: {undoUnit.Text}");

                undoUnit.Undo();
                isUndoing = false;
            }
        }

        public void Clear()
        {
            undoStack.Clear();
            redoStack.Clear();
            pendingUndoUnit = null;
            isUndoing = false;
        }

        public static UndoManager Create()
        {
            return new UndoManager();
        }

        public static void SetCurrent(UndoManager manager)
        {
            Instance = manager;
        }

        public static void ClearCurrent()
        {
            Instance = null;
        }
    }
}