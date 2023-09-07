using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using Frosty.Controls;
using Frosty.Core;

namespace TexturePlugin.Windows
{
    /// <summary>
    /// Interaction logic for ImportTextureWindow.xaml
    /// </summary>
    public partial class ImportTextureWindow : FrostyDockableWindow
    {
        private readonly string[] m_pixelFormats = {
        "NormalDXT1",
        "NormalDXN",
        "BC1A_SRGB",
        "BC1A_UNORM",
        "BC1_SRGB",
        "BC1_UNORM",
        "BC2_SRGB",
        "BC2_UNORM",
        "BC3_SRGB",
        "BC3_UNORM",
        "BC3A_UNORM",
        "BC3A_SRGB",
        "BC4_UNORM",
        "BC5_UNORM",
        "BC6U_FLOAT",
        "BC7",
        "BC7_SRGB",
        "BC7_UNORM",
        "R8_UNORM",
        "R16G16B16A16_FLOAT",
        "ARGB32F",
        "R32G32B32A32_FLOAT",
        "R9G9B9E5F",
        "R9G9B9E5_FLOAT",
        "R8G8B8A8_UNORM",
        "R8G8B8A8_SRGB",
        "B8G8R8A8_UNORM",
        "R10G10B10A2_UNORM",
        "L8",
        "L16",
        "ARGB8888",
        "R16G16_UNORM",
        "D16_UNORM" };
        public ImportTextureWindow(bool srgb)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;

            foreach(string pxFmt in m_pixelFormats)
            {
                bool valid = srgb 
                    ? pxFmt.Contains("SRGB") 
                    : !pxFmt.Contains("SRGB");

                if (!valid) continue;

                pixelFormatComboBox.Items.Add(pxFmt);
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public SharpDX.DXGI.Format GetSelectedPixelFormat()
        {
            switch(pixelFormatComboBox.SelectedItem)
            {
                //case "DXT1": return SharpDX.DXGI.Format.BC1_UNorm;
                case "NormalDXT1": return SharpDX.DXGI.Format.BC1_UNorm;
                case "NormalDXN": return SharpDX.DXGI.Format.BC5_UNorm;
                //case "DXT1A": return SharpDX.DXGI.Format.BC1_UNorm;
                case "BC1A_SRGB": return SharpDX.DXGI.Format.BC1_UNorm_SRgb;
                case "BC1A_UNORM": return SharpDX.DXGI.Format.BC1_UNorm;
                case "BC1_SRGB": return SharpDX.DXGI.Format.BC1_UNorm_SRgb;
                case "BC1_UNORM": return SharpDX.DXGI.Format.BC1_UNorm;
                case "BC2_SRGB": return SharpDX.DXGI.Format.BC2_UNorm_SRgb;
                case "BC2_UNORM": return SharpDX.DXGI.Format.BC2_UNorm;
                //case "DXT3": return SharpDX.DXGI.Format.BC2_UNorm;
                case "BC3_SRGB": return SharpDX.DXGI.Format.BC3_UNorm_SRgb;
                case "BC3_UNORM": return SharpDX.DXGI.Format.BC3_UNorm;
                case "BC3A_UNORM": return SharpDX.DXGI.Format.BC3_UNorm;
                case "BC3A_SRGB": return SharpDX.DXGI.Format.BC3_UNorm_SRgb;
                case "BC4_UNORM": return SharpDX.DXGI.Format.BC4_UNorm;
                //case "DXT5": return SharpDX.DXGI.Format.BC3_UNorm;
                //case "DXT5A": return SharpDX.DXGI.Format.BC3_UNorm;
                case "BC5_UNORM": return SharpDX.DXGI.Format.BC5_UNorm;
                case "BC6U_FLOAT": return SharpDX.DXGI.Format.BC6H_Uf16;
                case "BC7": return SharpDX.DXGI.Format.BC7_UNorm;
                case "BC7_SRGB": return SharpDX.DXGI.Format.BC7_UNorm_SRgb;
                case "BC7_UNORM": return SharpDX.DXGI.Format.BC7_UNorm;
                case "R8_UNORM": return SharpDX.DXGI.Format.R8_UNorm;
                case "R16G16B16A16_FLOAT": return SharpDX.DXGI.Format.R16G16B16A16_Float;
                case "ARGB32F": return SharpDX.DXGI.Format.R32G32B32A32_Float;
                case "R32G32B32A32_FLOAT": return SharpDX.DXGI.Format.R32G32B32A32_Float;
                case "R9G9B9E5F": return SharpDX.DXGI.Format.R9G9B9E5_Sharedexp;
                case "R9G9B9E5_FLOAT": return SharpDX.DXGI.Format.R9G9B9E5_Sharedexp;
                case "R8G8B8A8_UNORM": return SharpDX.DXGI.Format.R8G8B8A8_UNorm;
                case "R8G8B8A8_SRGB": return SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb;
                case "B8G8R8A8_UNORM": return SharpDX.DXGI.Format.B8G8R8A8_UNorm;
                case "R10G10B10A2_UNORM": return SharpDX.DXGI.Format.R10G10B10A2_UNorm;
                case "L8": return SharpDX.DXGI.Format.R8_UNorm;
                case "L16": return SharpDX.DXGI.Format.R16_UNorm;
                case "ARGB8888": return SharpDX.DXGI.Format.R8G8B8A8_UNorm;
                case "R16G16_UNORM": return SharpDX.DXGI.Format.R16G16_UNorm;
                case "D16_UNORM": return SharpDX.DXGI.Format.R16_UNorm;
                default: return SharpDX.DXGI.Format.Unknown;
            }
        }
    }
}
