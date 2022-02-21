using FrostySdk.IO;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Frosty.Core.Controls.Editors
{
    public class FrostyBlobEditor : FrostyTypeEditor<FrostyBlobControl>
    {
        public FrostyBlobEditor()
        {
            ValueProperty = FrostyBlobControl.ValueProperty;
        }
    }

    [TemplatePart(Name = PART_ExportButton, Type = typeof(Button))]
    public class FrostyBlobControl : Control
    {
        private const string PART_ExportButton = "PART_ExportButton";

        #region -- Properties --

        #region -- Value --
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(FrostyBlobControl), new FrameworkPropertyMetadata(null));
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        #endregion

        #endregion

        private Button exportButton;

        static FrostyBlobControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyBlobControl), new FrameworkPropertyMetadata(typeof(FrostyBlobControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            exportButton = GetTemplateChild(PART_ExportButton) as Button;
            exportButton.Click += ExportButton_Click;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save RAW data", "*.raw (RAW File)|*.raw", "Raw");
            if (sfd.ShowDialog())
            {
                List<byte> blobData = (List<byte>)Value;
                using (NativeWriter writer = new NativeWriter(new FileStream(sfd.FileName, FileMode.Create)))
                    writer.Write(blobData.ToArray());
            }
        }
    }
}
