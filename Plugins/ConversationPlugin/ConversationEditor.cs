using FrostySdk.Ebx;
using FrostySdk.Interfaces;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Frosty.Core.Controls;
using Frosty.Core;
using FrostySdk.Managers.Entries;

namespace ConversationPlugin
{
    class ConversationLineInfo
    {
        public string Speaker { get; private set; }
        public string Line { get; private set; }
        public string ToolTip { get; private set; }
        public object Data { get; private set; }
        public IEnumerable<ConversationLineInfo> Children => GetChildren();

        public ConversationLineInfo(dynamic data)
        {
            Data = data;
            Type type = data.GetType();

            if (type.Name == "Conversation")
            {
                Line = "Conversation";
                Speaker = "";
            }
            else if (type.Name == "ConversationLink")
            {
                Line = "Link";
                Speaker = "";
            }
            else
            {
                PointerRef pr = data.TextReference.Speaker;
                EbxAssetEntry entry = App.AssetManager.GetEbxEntry(pr.External.FileGuid);
                Speaker = (entry != null) ? entry.Filename : "No-one";

                Line = LocalizedStringDatabase.Current.GetString((uint)data.TextReference.String.StringId);
                ToolTip = Line;

                Line = Line.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            }
        }

        private IEnumerable<ConversationLineInfo> GetChildren()
        {
            dynamic data = Data;
            Type type = data.GetType();

            if (type.Name == "ConversationLink")
            {
                dynamic linkedLine = data.LinkedLine.Internal;
                foreach (PointerRef convLineRef in linkedLine.ChildNodes)
                {
                    dynamic convLine = convLineRef.Internal;
                    yield return new ConversationLineInfo(convLine);
                }
            }
            else
            {
                foreach (PointerRef convLineRef in data.ChildNodes)
                {
                    dynamic convLine = convLineRef.Internal;
                    yield return new ConversationLineInfo(convLine);
                }
            }
        }
    }

    [TemplatePart(Name = PART_ConvLinesTreeView, Type = typeof(TreeView))]
    public class ConversationEditor : FrostyAssetEditor
    {
        private const string PART_ConvLinesTreeView = "PART_ConvLinesTreeView";
        private TreeView convListTreeView;
        private bool firstTimeLoad = true;

        static ConversationEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConversationEditor), new FrameworkPropertyMetadata(typeof(ConversationEditor)));
        }

        public ConversationEditor(ILogger inLogger)
            : base(inLogger)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            convListTreeView = GetTemplateChild(PART_ConvLinesTreeView) as TreeView;
            convListTreeView.SelectedItemChanged += ConvListTreeView_SelectedItemChanged;
            Loaded += DAIConversationEditor_Loaded;
        }

        private void ConvListTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ConversationLineInfo info = convListTreeView.SelectedItem as ConversationLineInfo;
            if (info == null)
                return;

            FrostyPropertyGrid pg = GetTemplateChild("PART_AssetPropertyGrid") as FrostyPropertyGrid;
            pg.SetClass(info.Data);
        }

        private void DAIConversationEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstTimeLoad)
            {
                ConversationLineInfo rootLine = new ConversationLineInfo(asset.RootObject);
                convListTreeView.ItemsSource = new ConversationLineInfo[] { rootLine };

                firstTimeLoad = false;
            }
        }
    }
}
