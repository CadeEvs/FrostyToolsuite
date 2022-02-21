using Frosty.Core.Attributes;
using FrostySdk.Interfaces;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace FrostyCore
{
    public class FrostyLogger : ILogger, INotifyPropertyChanged
    {
        public string LogText => sb.ToString();
        private StringBuilder sb = new StringBuilder();

        public void Log(string text, params object[] vars)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            string category = "[Core] ";
            var attr = assembly.GetCustomAttribute<PluginDisplayNameAttribute>();

            if (attr != null)
                category = "[" + attr.DisplayName + "] ";

            sb.AppendLine(string.Format("[" + DateTime.Now.ToLongTimeString() + "]: " + category + text, vars));
            RaisePropertyChanged("LogText");
        }

        public void LogWarning(string text, params object[] vars)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            string category = "[Core] ";
            var attr = assembly.GetCustomAttribute<PluginDisplayNameAttribute>();

            if (attr != null)
                category = "[" + attr.DisplayName + "] ";

            sb.AppendLine(string.Format("[" + DateTime.Now.ToLongTimeString() + "]: " + category + "(WARNING) " + text, vars));
            RaisePropertyChanged("LogText");
        }

        public void LogError(string text, params object[] vars)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            string category = "[Core] ";
            var attr = assembly.GetCustomAttribute<PluginDisplayNameAttribute>();

            if (attr != null)
                category = "[" + attr.DisplayName + "] ";

            sb.AppendLine(string.Format("[" + DateTime.Now.ToLongTimeString() + "]: " + category + "(ERROR) " + text, vars));
            RaisePropertyChanged("LogText");
        }

        public void AddBinding(UIElement elementToBind, DependencyProperty propertyToBind)
        {
            Binding b = new Binding("LogText")
            {
                Source = this,
                Mode = BindingMode.OneWay
            };

            BindingOperations.SetBinding(elementToBind, propertyToBind, b);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
