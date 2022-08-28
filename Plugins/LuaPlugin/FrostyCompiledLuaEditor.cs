using System;
using FrostySdk.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using FrostySdk.Managers;
using Frosty.Core.Controls;
using Frosty.Core;
using FrostySdk.IO;

namespace LuaPlugin
{
    [TemplatePart(Name = PART_TextEditor, Type = typeof(CodeEditor.CodeEditor))]
    [TemplatePart(Name = PART_ExportButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_ImportButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_CompileButton, Type = typeof(Button))]
    public class FrostyCompiledLuaEditor : FrostyAssetEditor
    {
        private const string PART_TextEditor = "PART_TextEditor";
        private const string PART_ExportButton = "PART_ExportButton";
        private const string PART_ImportButton = "PART_ImportButton";
        private const string PART_CompileButton = "PART_CompileButton";

        private CodeEditor.CodeEditor TextEditor;
        private Button ExportButton;
        private Button ImportButton;
        private Button CompileButton;

        private ResAssetEntry ResEntry;
        private CompiledLuaResource Resource;

        static FrostyCompiledLuaEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyCompiledLuaEditor), new FrameworkPropertyMetadata(typeof(FrostyCompiledLuaEditor)));
        }

        public FrostyCompiledLuaEditor(ILogger inLogger) : base(inLogger)
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ulong resID = ((dynamic)RootObject).CompiledLuaResource;
            ResEntry = App.AssetManager.GetResEntry(resID);
            Resource = App.AssetManager.GetResAs<CompiledLuaResource>(ResEntry);

            TextEditor = GetTemplateChild(PART_TextEditor) as CodeEditor.CodeEditor;
            try
            {
                TextEditor.Text = Resource.DecompileBytecode();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception decompiling Lua: \n\n" + ex.ToString());
            }

            ExportButton = GetTemplateChild(PART_ExportButton) as Button;
            ExportButton.Click += ExportButton_Click;

            ImportButton = GetTemplateChild(PART_ImportButton) as Button;
            ImportButton.Click += ImportButton_Click;

            CompileButton = GetTemplateChild(PART_CompileButton) as Button;
            CompileButton.Click += CompileButton_Click;

            TextEditor.IndentText();
        }

        private void UpdateLua(string[] newSource)
        {
            try
            {
                Resource.CompileSource(newSource, logger);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception compiling Lua: \n\n" + ex.ToString());
                return;
            }
            try
            {
                TextEditor.Text = Resource.DecompileBytecode(logger);
                TextEditor.IndentText();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception decompiling Lua: \n\n" + ex.ToString());
                return;
            }

            // Modify resource
            App.AssetManager.RevertAsset(AssetEntry, dataOnly: true);
            App.AssetManager.ModifyRes(ResEntry.ResRid, Resource);

            AssetEntry.LinkedAssets.Add(ResEntry);
            InvokeOnAssetModified();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            string filter = "Lua file (*.lua)|*.lua";
#if DEBUG
            filter += "|Compiled Frosty Lua Resource (*.luares)|*.luares";
#endif

            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save Lua file", filter, "Lua", AssetEntry.Filename);
            if (sfd.ShowDialog())
            {
                if (sfd.FilterIndex == 1) // Lua source
                {
                    try
                    {
                        File.WriteAllText(sfd.FileName, Resource.DecompileBytecode(logger));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception decompiling Lua: \n\n" + ex.ToString());
                    }
                }
                else if (sfd.FilterIndex == 2) // Raw resource
                {
                    // TODO: Maybe show task dialog?
                    using (NativeWriter writer = new NativeWriter(new FileStream(sfd.FileName, FileMode.Create)))
                    {
                        writer.Write(Resource.SaveBytes());
                    }
                }
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            string filter = "Lua file (*.lua)|*.lua";
#if DEBUG
            filter += "|Compiled Frosty Lua Resource (*.luares)|*.luares";
#endif

            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("open Lua file", filter, "Lua");
            if (ofd.ShowDialog())
            {
                if (ofd.FilterIndex == 1) // Lua source
                {
                    UpdateLua(File.ReadAllLines(ofd.FileName));
                    TextEditor.IndentText();
                }
                else if (ofd.FilterIndex == 2) // Raw resource
                {
                    App.AssetManager.ModifyRes(ResEntry.ResRid, File.ReadAllBytes(ofd.FileName));
                    AssetEntry.LinkedAssets.Add(ResEntry);
                }
            }
        }

        private void CompileButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateLua(TextEditor.Text.Split('\n'));
        }

        private void TextEditor_TextChanged(object sender, RoutedEventArgs e)
        {
            string[] lines = TextEditor.Text.Split('\n');
            UpdateLua(lines);
        }
    }
}
