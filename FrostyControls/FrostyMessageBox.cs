using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Frosty.Controls
{
    internal class MessageBoxClickCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Button btn = parameter as Button;
            FrostyMessageBox parentWin = Window.GetWindow(btn) as FrostyMessageBox;

            string buttonName = (string)btn.Content;

            MessageBoxResult result = MessageBoxResult.None;
            if (buttonName == "OK") result = MessageBoxResult.OK;
            else if (buttonName == "No") result = MessageBoxResult.No;
            else if (buttonName == "Yes") result = MessageBoxResult.Yes;
            else result = MessageBoxResult.Cancel;

            parentWin.RequestClose(result);
        }
    }

    public class FrostyMessageBox : FrostyDockableWindow
    {
        #region -- Properties --

        #region -- Text --
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(FrostyMessageBox), new PropertyMetadata(""));
        protected string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #region -- Alignment --
        public static readonly DependencyProperty AlignmentProperty = DependencyProperty.Register("Alignment", typeof(TextAlignment), typeof(FrostyMessageBox), new PropertyMetadata(TextAlignment.Left));
        protected TextAlignment Alignment
        {
            get => (TextAlignment)GetValue(AlignmentProperty);
            set => SetValue(AlignmentProperty, value);
        }
        #endregion

        #region -- Buttons --
        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register("Buttons", typeof(MessageBoxButton), typeof(FrostyMessageBox), new PropertyMetadata(MessageBoxButton.OK));
        protected MessageBoxButton Buttons
        {
            get => (MessageBoxButton)GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }
        #endregion

        #region -- RememberActionPrompt --
        public static readonly DependencyProperty RememberActionPromptProperty = DependencyProperty.Register("RememberActionPrompt", typeof(bool), typeof(FrostyMessageBox), new PropertyMetadata(false));
        protected bool RememberActionPrompt
        {
            get => (bool)GetValue(RememberActionPromptProperty);
            set => SetValue(RememberActionPromptProperty, value);
        }
        #endregion

        #region -- RememberActionResult --
        public static readonly DependencyProperty RememberActionResultProperty = DependencyProperty.Register("RememberActionResult", typeof(bool), typeof(FrostyMessageBox), new PropertyMetadata(false));
        protected bool RememberActionResult
        {
            get => (bool)GetValue(RememberActionResultProperty);
            set => SetValue(RememberActionResultProperty, value);
        }
        #endregion

        #endregion

        public MessageBoxResult MessageBoxResult { get; set; }

        public FrostyMessageBox()
        {
            Topmost = true;
            ShowInTaskbar = false;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MessageBoxResult = MessageBoxResult.Cancel;
            SizeToContent = SizeToContent.Height;

            MaxWidth = 410;
            //MinWidth = 175;
            MinHeight = 175;

            Window win = Application.Current.MainWindow;
            Icon = win.Icon;
        }

        public void RequestClose(MessageBoxResult result)
        {
            MessageBoxResult = result;
            Close();
        }

        public static MessageBoxResult Show(string text)
        {
            bool rememberAction = false;
            return ShowInternal(text, "", MessageBoxButton.OK, false, ref rememberAction);
        }

        public static MessageBoxResult Show(string text, string title)
        {
            bool rememberAction = false;
            return ShowInternal(text, title, MessageBoxButton.OK, false, ref rememberAction);
        }

        public static MessageBoxResult Show(string text, string title, MessageBoxButton button)
        {
            bool rememberAction = false;
            return ShowInternal(text, title, button, false, ref rememberAction);
        }

        public static MessageBoxResult Show(string text, string title, MessageBoxButton button, ref bool rememberAction)
        {
            return ShowInternal(text, title, button, true, ref rememberAction);
        }

        private static MessageBoxResult ShowInternal(string text, string title, MessageBoxButton button, bool rememberActionPrompt, ref bool rememberAction)
        {
            MessageBoxResult msgBoxResult = MessageBoxResult.None;
            TextAlignment alignment = TextAlignment.Center;
            if (text.Contains("\r\n"))
                alignment = TextAlignment.Left;

            if (System.Threading.Thread.CurrentThread.GetApartmentState() != System.Threading.ApartmentState.STA)
            {
                bool result = false;
                bool rememberActionResult = false;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    FrostyMessageBox window = new FrostyMessageBox
                    {
                        Text = text,
                        Title = title,
                        Buttons = button,
                        Alignment = alignment,
                        RememberActionPrompt = rememberActionPrompt,
                        RememberActionResult = rememberActionResult
                    };

                    window.ShowDialog();
                    result = true;
                    msgBoxResult = window.MessageBoxResult;
                });

                while (!result)
                    System.Threading.Thread.Sleep(10);

                rememberAction = rememberActionResult;
            }
            else
            {
                FrostyMessageBox window = new FrostyMessageBox
                {
                    Text = text,
                    Title = title,
                    Buttons = button,
                    Alignment = alignment,
                    RememberActionPrompt = rememberActionPrompt,
                    RememberActionResult = rememberAction
                };
                window.ShowDialog();
                msgBoxResult = window.MessageBoxResult;
            }

            return msgBoxResult;
        }
    }
}
