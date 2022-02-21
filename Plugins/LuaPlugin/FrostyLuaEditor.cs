using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace LuaPlugin
{
    [TemplatePart(Name = PART_TextEditor, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_ExportButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_ImportButton, Type = typeof(Button))]
    public class FrostyLuaEditor : FrostyAssetEditor
    {
        private const string PART_TextEditor = "PART_TextEditor";
        private const string PART_ExportButton = "PART_ExportButton";
        private const string PART_ImportButton = "PART_ImportButton";

        private CodeEditor.CodeEditor TextEditor;
        private Button ExportButton;
        private Button ImportButton;

        public FrostyLuaEditor(ILogger inLogger)
            : base(inLogger)
        {

        }

        static FrostyLuaEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyLuaEditor), new FrameworkPropertyMetadata(typeof(FrostyLuaEditor)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TextEditor = GetTemplateChild(PART_TextEditor) as CodeEditor.CodeEditor;
            TextEditor.Text = ((dynamic)RootObject).Script;
            TextEditor.TextChanged += TextEditor_TextChanged;

            ExportButton = GetTemplateChild(PART_ExportButton) as Button;
            ExportButton.Click += ExportButton_Click;

            ImportButton = GetTemplateChild(PART_ImportButton) as Button;
            ImportButton.Click += ImportButton_Click;

            TextEditor.IndentText();
        }

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save Lua file", "Lua file (*.lua)|*.lua", "Lua", AssetEntry.Filename);
            if (sfd.ShowDialog())
            {
                File.WriteAllText(sfd.FileName, TextEditor.Text);
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Open Lua file", "Lua file (*.lua)|*.lua", "Lua");
            if (ofd.ShowDialog())
            {
                TextEditor.Text = File.ReadAllText(ofd.FileName);
                TextEditor.IndentText();
            }
        }

        private void TextEditor_TextChanged(object sender, RoutedEventArgs e)
        {
            ((dynamic)RootObject).Script = TextEditor.Text;
            AssetModified = true;
        }
    }
}
