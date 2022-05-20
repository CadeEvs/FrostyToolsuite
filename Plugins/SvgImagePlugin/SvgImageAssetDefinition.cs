using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Media;

namespace SvgImagePlugin
{
    public class SvgImageAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/SvgImagePlugin;component/Images/SvgFileType.png") as ImageSource;
        public override void GetSupportedExportTypes(List<AssetExportType> exportTypes)
        {
            exportTypes.Add(new AssetExportType("svg", "Scalable Vector Graphics"));

            base.GetSupportedExportTypes(exportTypes);
        }

        public override ImageSource GetIcon()
        {
            return imageSource;
        }

        public override bool Export(EbxAssetEntry entry, string path, string filterType)
        {
            if (!base.Export(entry, path, filterType))
            {
                if (filterType == "svg")
                {
                    EbxAsset asset = App.AssetManager.GetEbx(entry);
                    dynamic textureAsset = (dynamic)asset.RootObject;

                    ResAssetEntry resEntry = App.AssetManager.GetResEntry(textureAsset.Resource);
                    SvgImage image = App.AssetManager.GetResAs<SvgImage>(resEntry);

                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    {
                        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "no"));
                        xmlDoc.AppendChild(xmlDoc.CreateDocumentType("svg", "-//W3C//DTD SVG 20010904//EN", "http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd", ""));

                        System.Xml.XmlElement svgElem = xmlDoc.CreateNode(System.Xml.XmlNodeType.Element, "svg", "") as System.Xml.XmlElement;
                        svgElem.SetAttribute("version", "1.1");
                        svgElem.SetAttribute("id", "Frosty Editor v" + Assembly.GetEntryAssembly().GetName().Version.ToString());
                        svgElem.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
                        svgElem.SetAttribute("xlink", "xmlns", "http://www.w3.org/1999/xlink");
                        svgElem.SetAttribute("viewBox", "0 0 " + (int)image.Width + " " + (int)image.Height);
                        xmlDoc.AppendChild(svgElem);

                        foreach (SvgShape shape in image.Shapes)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (SvgPath svgPath in shape.Paths)
                            {
                                sb.Append("M " + svgPath.Points[0].X.ToString("F3") + "," + svgPath.Points[0].Y.ToString("F3") + " ");
                                sb.Append("C ");
                                for (int i = 1; i < svgPath.Points.Count; i++)
                                    sb.Append(svgPath.Points[i].X.ToString("F3") + "," + svgPath.Points[i].Y.ToString("F3") + " ");

                                if (svgPath.Closed)
                                    sb.Append("Z ");
                            }

                            System.Xml.XmlElement pathElem = xmlDoc.CreateNode(System.Xml.XmlNodeType.Element, "path", "") as System.Xml.XmlElement;
                            pathElem.SetAttribute("stroke", "none");
                            pathElem.SetAttribute("fill", "none");

                            if (shape.Stroke)
                            {
                                pathElem.SetAttribute("stroke", "#" + shape.StrokeColor.ToString("x8"));
                                pathElem.SetAttribute("stroke-width", shape.Thickness.ToString());
                            }
                            if (shape.Fill)
                                pathElem.SetAttribute("fill", "#" + shape.FillColor.ToString("x8"));

                            pathElem.SetAttribute("d", sb.ToString());
                            svgElem.AppendChild(pathElem);
                        }

                        xmlDoc.Save(new FileStream(path, FileMode.Create));
                    }
                    return true;
                }
            }

            return false;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new FrostySvgImageEditor(logger);
        }
    }
}
