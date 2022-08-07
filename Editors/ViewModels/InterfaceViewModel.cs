using Frosty.Core;
using LevelEditorPlugin.Converters;
using LevelEditorPlugin.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DataField = FrostySdk.Ebx.DataField;
using DynamicEvent = FrostySdk.Ebx.DynamicEvent;

namespace LevelEditorPlugin.Editors
{
    public interface ISchematicsInterfaceProvider
    {
        InterfaceDescriptor Interface { get; }

        void InterfacePropertyPushed(int propertyHash, string newValue);
        void InterfaceEventPushed(int eventHash);

        event EventHandler<InterfaceOutputPropertyChangedEventArgs> OnInterfaceOutputPropertyChanged;
        event EventHandler<InterfaceOutputEventTriggeredEventArgs> OnInterfaceOutputEventTriggered;
    }

    public class InputPropertyViewModel : INotifyPropertyChanged
    {
        public string Name => dataField.Name;
        public int Hash => dataField.Id;
        public string Value
        {
            get => dataValue;
            set
            {
                if (dataValue != value)
                {
                    dataValue = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ICommand PushCommand => pushCommand;

        private DataField dataField;
        private string dataValue;
        private ICommand pushCommand;

        public InputPropertyViewModel(DataField inDataField, ICommand interfacePushCommand)
        {
            dataField = inDataField;
            dataValue = dataField.Value;
            pushCommand = new RelayCommand((o) => { interfacePushCommand.Execute(this); });
        }

        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class OutputPropertyViewModel : INotifyPropertyChanged
    {
        public string Name => dataField.Name;
        public int Hash => dataField.Id;
        public string Value
        {
            get => dataValue;
            set
            {
                if (dataValue != value)
                {
                    dataValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DataField dataField;
        private string dataValue;

        public OutputPropertyViewModel(DataField inDataField)
        {
            dataField = inDataField;
            dataValue = dataField.Value;
        }

        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class InputEventViewModel : INotifyPropertyChanged
    {
        public string Name => dynamicEvent.Name;
        public int Hash => dynamicEvent.Id;
        public ICommand PushCommand => pushCommand;

        private DynamicEvent dynamicEvent;
        private ICommand pushCommand;

        public InputEventViewModel(DynamicEvent inEvent, ICommand interfacePushCommand)
        {
            dynamicEvent = inEvent;
            pushCommand = new RelayCommand((o) => { interfacePushCommand.Execute(this); });
        }

        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class OutputEventViewModel : INotifyPropertyChanged
    {
        public string Name => dynamicEvent.Name;
        public int Hash => dynamicEvent.Id;
        public bool Triggered
        {
            get => triggered;
            set
            {
                triggered = value;
                if (triggered)
                {
                    alpha = 1.0f;
                    timer.Change(0, 10);
                }
            }
        }
        public float Alpha
        {
            get => alpha;
            set
            {
                if (alpha != value)
                {
                    alpha = value;
                    if (alpha <= 0.0f)
                    {
                        triggered = false;
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        private DynamicEvent dynamicEvent;
        private bool triggered;
        private Timer timer;
        private float alpha;

        public OutputEventViewModel(DynamicEvent inEvent)
        {
            dynamicEvent = inEvent;
            timer = new Timer(OnTimer, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void OnTimer(object state)
        {
            Alpha -= 0.01f;
        }

        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class InterfaceViewModel : Controls.IDockableItem, INotifyPropertyChanged
    {
        public string Header => "Interface";
        public string UniqueId => "UID_LevelEditor_Interface";
        public string Icon => "Images/Interface.png";
        public IEnumerable<object> InterfaceVars => interfaceVars;

        private ISchematicsInterfaceProvider owner;
        private List<object> interfaceVars = new List<object>();

        public InterfaceViewModel(ISchematicsInterfaceProvider inOwner)
        {
            owner = inOwner;
            owner.OnInterfaceOutputPropertyChanged += InterfaceOutputPropertyChanged;
            owner.OnInterfaceOutputEventTriggered += InterfaceOutputEventTriggered;
            if (owner.Interface != null)
            {
                foreach (DataField dataField in owner.Interface.Data.Fields.Where(df => df.AccessType == FrostySdk.Ebx.FieldAccessType.FieldAccessType_Target || df.AccessType == FrostySdk.Ebx.FieldAccessType.FieldAccessType_SourceAndTarget))
                {
                    interfaceVars.Add(new InputPropertyViewModel(dataField, new RelayCommand(OnPushPropertyCommand)));
                }

                foreach (DynamicEvent evt in owner.Interface.Data.InputEvents)
                {
                    interfaceVars.Add(new InputEventViewModel(evt, new RelayCommand(OnPushEventCommand)));
                }

                // @todo: input links

                foreach (DataField dataField in owner.Interface.Data.Fields.Where(df => df.AccessType == FrostySdk.Ebx.FieldAccessType.FieldAccessType_Source || df.AccessType == FrostySdk.Ebx.FieldAccessType.FieldAccessType_SourceAndTarget))
                {
                    interfaceVars.Add(new OutputPropertyViewModel(dataField));
                }

                foreach (DynamicEvent evt in owner.Interface.Data.OutputEvents)
                {
                    interfaceVars.Add(new OutputEventViewModel(evt));
                }

                // @todo: output links
            }
        }

        private void InterfaceOutputEventTriggered(object sender, InterfaceOutputEventTriggeredEventArgs e)
        {
            object value = interfaceVars.FirstOrDefault(i => (i is OutputEventViewModel) && (i as OutputEventViewModel).Hash == e.EventHash);
            if (value != null)
            {
                OutputEventViewModel outputEvent = value as OutputEventViewModel;
                outputEvent.Triggered = true;
            }
        }

        private void InterfaceOutputPropertyChanged(object sender, InterfaceOutputPropertyChangedEventArgs e)
        {
            object value = interfaceVars.FirstOrDefault(i => (i is OutputPropertyViewModel) && (i as OutputPropertyViewModel).Hash == e.PropertyHash);
            if (value != null)
            {
                OutputPropertyViewModel outputProperty = value as OutputPropertyViewModel;
                outputProperty.Value = (string)new DataFieldToValueConverter().ConvertBack(e.NewValue, typeof(string), null, null);
            }
        }

        private void OnPushPropertyCommand(object o)
        {
            InputPropertyViewModel property = o as InputPropertyViewModel;
            owner.InterfacePropertyPushed(property.Hash, property.Value);
        }

        private void OnPushEventCommand(object o)
        {
            InputEventViewModel evt = o as InputEventViewModel;
            owner.InterfaceEventPushed(evt.Hash);
        }

        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
