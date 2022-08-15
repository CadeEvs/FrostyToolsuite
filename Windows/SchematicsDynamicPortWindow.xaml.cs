using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Frosty.Controls;
using Frosty.Core;
using FrostySdk.Attributes;
using FrostySdk.Ebx;
using FrostySdk.IO;
using LevelEditorPlugin.Controls;
using LevelEditorPlugin.Entities;

namespace LevelEditorPlugin.Windows
{
    public enum PortType
    {
        Link,
        Event,
        Property
    }
    
    [System.ComponentModel.DisplayName("Port")]
    public class DynamicPortSettings
    {
        [EbxFieldMeta(EbxFieldType.String)]
        public string Name { get; set; } = "";
        
        [EbxFieldMeta(EbxFieldType.Enum)]
        public PortType Type { get; set; } = PortType.Property;
        
        [EbxFieldMeta(EbxFieldType.Enum)]
        public Direction Direction { get; set; } = Direction.In;
    }

    public partial class SchematicsDynamicPortWindow : FrostyDockableWindow
    {
        public ICommand CancelButtonClickedCommand => new RelayCommand(CancelButtonClicked);
        public ICommand AddButtonClickedCommand => new RelayCommand(AddButtonClicked);

        public object Options => m_options;

        private object m_options;
        
        public SchematicsDynamicPortWindow()
        {
            InitializeComponent();

            m_options = new DynamicPortSettings();
        }

        private void CancelButtonClicked(object obj)
        {
            Close();
        }
        private void AddButtonClicked(object obj)
        {
            DialogResult = true;
            Close();
        }
    }
}