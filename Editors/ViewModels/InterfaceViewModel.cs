using Frosty.Core;
using LevelEditorPlugin.Converters;
using LevelEditorPlugin.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FrostySdk.Ebx;

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
        public string Name => m_dataField.Name;
        public int Hash => m_dataField.Id;
        public string Value
        {
            get => m_dataValue;
            set
            {
                if (m_dataValue != value)
                {
                    m_dataValue = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ICommand PushCommand => m_pushCommand;

        private DataField m_dataField;
        private string m_dataValue;
        private ICommand m_pushCommand;

        public InputPropertyViewModel(DataField inDataField, ICommand interfacePushCommand)
        {
            m_dataField = inDataField;
            m_dataValue = m_dataField.Value;
            m_pushCommand = new RelayCommand((o) => { interfacePushCommand.Execute(this); });
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
        public string Name => m_dataField.Name;
        public int Hash => m_dataField.Id;
        public string Value
        {
            get => m_dataValue;
            set
            {
                if (m_dataValue != value)
                {
                    m_dataValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DataField m_dataField;
        private string m_dataValue;

        public OutputPropertyViewModel(DataField inDataField)
        {
            m_dataField = inDataField;
            m_dataValue = m_dataField.Value;
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
        public string Name => m_dynamicEvent.Name;
        public int Hash => m_dynamicEvent.Id;
        public ICommand PushCommand => m_pushCommand;

        private DynamicEvent m_dynamicEvent;
        private ICommand m_pushCommand;

        public InputEventViewModel(DynamicEvent inEvent, ICommand interfacePushCommand)
        {
            m_dynamicEvent = inEvent;
            m_pushCommand = new RelayCommand((o) => { interfacePushCommand.Execute(this); });
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
        public string Name => m_dynamicEvent.Name;
        public int Hash => m_dynamicEvent.Id;
        public bool Triggered
        {
            get => m_triggered;
            set
            {
                m_triggered = value;
                if (m_triggered)
                {
                    m_alpha = 1.0f;
                    m_timer.Change(0, 10);
                }
            }
        }
        public float Alpha
        {
            get => m_alpha;
            set
            {
                if (m_alpha != value)
                {
                    m_alpha = value;
                    if (m_alpha <= 0.0f)
                    {
                        m_triggered = false;
                        m_timer.Change(Timeout.Infinite, Timeout.Infinite);
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        private DynamicEvent m_dynamicEvent;
        private bool m_triggered;
        private Timer m_timer;
        private float m_alpha;

        public OutputEventViewModel(DynamicEvent inEvent)
        {
            m_dynamicEvent = inEvent;
            m_timer = new Timer(OnTimer, null, Timeout.Infinite, Timeout.Infinite);
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

    public class InputLinkViewModel : INotifyPropertyChanged
    {
        public string Name => m_dynamicLink.Name;
        public int Hash => m_dynamicLink.Id;

        private DynamicLink m_dynamicLink;

        public InputLinkViewModel(DynamicLink inLink)
        {
            m_dynamicLink = inLink;
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
    
    public class OutputLinkViewModel : INotifyPropertyChanged
    {
        public string Name => m_dynamicLink.Name;
        public int Hash => m_dynamicLink.Id;

        private DynamicLink m_dynamicLink;

        public OutputLinkViewModel(DynamicLink inLink)
        {
            m_dynamicLink = inLink;
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
        public IEnumerable<object> InterfaceVars => m_interfaceVars;

        private ISchematicsInterfaceProvider m_owner;
        private List<object> m_interfaceVars = new List<object>();

        public InterfaceViewModel(ISchematicsInterfaceProvider inOwner)
        {
            m_owner = inOwner;
            m_owner.OnInterfaceOutputPropertyChanged += InterfaceOutputPropertyChanged;
            m_owner.OnInterfaceOutputEventTriggered += InterfaceOutputEventTriggered;
            if (m_owner.Interface != null)
            {
                // input property
                foreach (DataField dataField in m_owner.Interface.Data.Fields.Where(df => df.AccessType == FrostySdk.Ebx.FieldAccessType.FieldAccessType_Target || df.AccessType == FrostySdk.Ebx.FieldAccessType.FieldAccessType_SourceAndTarget))
                {
                    m_interfaceVars.Add(new InputPropertyViewModel(dataField, new RelayCommand(OnPushPropertyCommand)));
                }
                // input event
                foreach (DynamicEvent dynamicEvent in m_owner.Interface.Data.InputEvents)
                {
                    m_interfaceVars.Add(new InputEventViewModel(dynamicEvent, new RelayCommand(OnPushEventCommand)));
                }
                // input link
                foreach (DynamicLink dynamicLink in m_owner.Interface.Data.InputLinks)
                {
                    m_interfaceVars.Add(new InputLinkViewModel(dynamicLink));
                }

                // output property
                foreach (DataField dataField in m_owner.Interface.Data.Fields.Where(df => df.AccessType == FrostySdk.Ebx.FieldAccessType.FieldAccessType_Source || df.AccessType == FrostySdk.Ebx.FieldAccessType.FieldAccessType_SourceAndTarget))
                {
                    m_interfaceVars.Add(new OutputPropertyViewModel(dataField));
                }
                // output event
                foreach (DynamicEvent dynamicEvent in m_owner.Interface.Data.OutputEvents)
                {
                    m_interfaceVars.Add(new OutputEventViewModel(dynamicEvent));
                }
                // output link
                foreach (DynamicLink dynamicLink in m_owner.Interface.Data.OutputLinks)
                {
                    m_interfaceVars.Add(new OutputLinkViewModel(dynamicLink));
                }
            }
        }

        private void InterfaceOutputEventTriggered(object sender, InterfaceOutputEventTriggeredEventArgs e)
        {
            object value = m_interfaceVars.FirstOrDefault(i => (i is OutputEventViewModel) && (i as OutputEventViewModel).Hash == e.EventHash);
            if (value != null)
            {
                OutputEventViewModel outputEvent = value as OutputEventViewModel;
                outputEvent.Triggered = true;
            }
        }

        private void InterfaceOutputPropertyChanged(object sender, InterfaceOutputPropertyChangedEventArgs e)
        {
            object value = m_interfaceVars.FirstOrDefault(i => (i is OutputPropertyViewModel) && (i as OutputPropertyViewModel).Hash == e.PropertyHash);
            if (value != null)
            {
                OutputPropertyViewModel outputProperty = value as OutputPropertyViewModel;
                outputProperty.Value = (string)new DataFieldToValueConverter().ConvertBack(e.NewValue, typeof(string), null, null);
            }
        }

        private void OnPushPropertyCommand(object o)
        {
            InputPropertyViewModel property = o as InputPropertyViewModel;
            m_owner.InterfacePropertyPushed(property.Hash, property.Value);
        }

        private void OnPushEventCommand(object o)
        {
            InputEventViewModel evt = o as InputEventViewModel;
            m_owner.InterfaceEventPushed(evt.Hash);
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
