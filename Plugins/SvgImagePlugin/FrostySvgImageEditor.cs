using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrostySdk.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Reflection;
using Path = System.Windows.Shapes.Path;
using FrostySdk.Managers;
using System.Text.RegularExpressions;
using System.Xml;
using Frosty.Core.Controls;
using Frosty.Core;
using Frosty.Core.Windows;
using Vector2 = SharpDX.Vector2;

namespace SvgImagePlugin
{
    [TemplatePart(Name = PART_Canvas, Type = typeof(Viewbox))]
    public class FrostySvgImageEditor : FrostyAssetEditor
    {
        private const string PART_Canvas = "PART_Canvas";

        #region -- GridVisible --
        public static readonly DependencyProperty GridVisibleProperty = DependencyProperty.Register("GridVisible", typeof(bool), typeof(FrostySvgImageEditor), new FrameworkPropertyMetadata(true));
        public bool GridVisible
        {
            get => (bool)GetValue(GridVisibleProperty);
            set => SetValue(GridVisibleProperty, value);
        }
        #endregion

        private Viewbox canvas;
        private SvgImage image;
        private bool firstTimeLoad = true;

        static FrostySvgImageEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostySvgImageEditor), new FrameworkPropertyMetadata(typeof(FrostySvgImageEditor)));
        }

        public FrostySvgImageEditor(ILogger inLogger) 
            : base(inLogger)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            canvas = GetTemplateChild(PART_Canvas) as Viewbox;
            Loaded += FrostySvgImageEditor_Loaded;
        }

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>()
            {
                new ToolbarItem("Export", "Export SVG", "Images/Export.png", new RelayCommand((object state) => { ExportButton_Click(this, new RoutedEventArgs()); })),
                new ToolbarItem("Import", "Import SVG", "Images/Import.png", new RelayCommand((object state) => { ImportButton_Click(this, new RoutedEventArgs()); })),
            };
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Open SVG", "*.svg (SVG File)|*.svg", "Svg");
            if (ofd.ShowDialog())
            {
                EbxAssetEntry assetEntry = AssetEntry as EbxAssetEntry;
                ulong resRid = ((dynamic)RootObject).Resource;
                bool bFailed = false;
                int warningCount = 0;

                FrostyTaskWindow.Show("Importing SVG", "", (task) =>
                {
                    try
                    {
                        // revert modified data
                        //App.AssetManager.RevertAsset(assetEntry, dataOnly: true);

                        System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                        xmlDoc.Load(new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read));

                        float width = 0.0f;
                        float height = 0.0f;

                        // no width/height defined
                        if (!xmlDoc["svg"].HasAttribute("viewBox"))
                        {
                            if (!xmlDoc["svg"].HasAttribute("width") && !xmlDoc["svg"].HasAttribute("height"))
                            {
                                bFailed = true;
                                logger.LogError("SVG must either have a viewBox or width and height specified.");
                                return;
                            }
                            else
                            {
                                if (!float.TryParse(xmlDoc["svg"].Attributes["width"].Value, out width))
                                {
                                    bFailed = true;
                                    logger.LogError("Unable to obtain width, was in an invalid format");
                                    return;
                                }
                                if (!float.TryParse(xmlDoc["svg"].Attributes["height"].Value, out height))
                                {
                                    bFailed = true;
                                    logger.LogError("Unable to obtain height, was in an invalid format");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            string viewBox = xmlDoc["svg"].Attributes["viewBox"].Value;
                            string[] values = viewBox.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            // obtain new width and height
                            if (!float.TryParse(values[2], out width))
                            {
                                bFailed = true;
                                logger.LogError("Unable to obtain width, was in an invalid format");
                                return;
                            }
                            if (!float.TryParse(values[3], out height))
                            {
                                bFailed = true;
                                logger.LogError("Unable to obtain height, was in an invalid format");
                                return;
                            }
                        }

                        image = new SvgImage(width, height);

                        // iterate paths
                        IterateNodesAndCreatePaths(xmlDoc["svg"], ref warningCount);

                        ResAssetEntry resEntry = App.AssetManager.GetResEntry(resRid);
                        App.AssetManager.ModifyRes(resRid, image);

                        assetEntry.LinkAsset(resEntry);
                    }
                    catch (Exception)
                    {
                        bFailed = true;
                        logger.Log("Failed to import {0} due to malformed data", ofd.FileName);
                    }
                });

                if (!bFailed)
                {
                    InvokeOnAssetModified();
                    if (warningCount == 0) { logger.Log("{0} successfully imported", ofd.FileName); }
                    else { logger.Log("{0} imported with {1} warning(s)", ofd.FileName, warningCount); }
                }

                UpdateCanvas();
            }
        }

        private void IterateNodesAndCreatePaths(System.Xml.XmlNode parentNode, ref int warningCount)
        {
            foreach (System.Xml.XmlNode node in parentNode)
            {
                if (node is XmlElement subElem)
                {
                    if (subElem.Name == "g")
                    {
                        IterateNodesAndCreatePaths(subElem, ref warningCount);
                    }
                    else if (subElem.Name == "path")
                    {
                        float thickness = 1.0f;
                        uint fillColor = 0;
                        uint strokeColor = 0;

                        if (subElem.HasAttribute("stroke-width"))
                            thickness = float.Parse(subElem.Attributes["stroke-width"].Value);

                        if (subElem.HasAttribute("fill") && subElem.Attributes["fill"].Value != "none")
                            fillColor = ConvertToColor(subElem.Attributes["fill"].Value);

                        if (subElem.HasAttribute("stroke") && subElem.Attributes["stroke"].Value != "none")
                            strokeColor = ConvertToColor(subElem.Attributes["stroke"].Value);

                        //if (fillColor == 0 && strokeColor == 0)
                        //    fillColor = 0xff000000;

                        List<SvgPath> paths = new List<SvgPath>();

                        char[] supportedTokens = new char[] { 'M', 'C', 'Z', 'm', 'c', 'z', 'L', 'l', 'V', 'v', 'H', 'h', 'S', 's', 'A', 'a' };
                        string dataText = subElem.Attributes["d"].Value;

                        List<char> tokens = new List<char>();
                        List<string> strings = new List<string>();

                        foreach (char c in dataText)
                        {
                            if (supportedTokens.Contains(c))
                            {
                                tokens.Add(c);
                                strings.Add("");

                                continue;
                            }

                            strings[strings.Count - 1] += c;
                        }

                        SvgPath path = new SvgPath();
                        Vector2 relPos = new Vector2();

                        Regex regEx = new Regex("-*[0-9]+(([e]{1}[-]*([0-9]{0,12}))|([.]*(([0-9]{0,12}[ ,]{1})|([0-9]{0,3}))))");
                        for (int i = 0; i < tokens.Count; i++)
                        {
                            char token = tokens[i];

                            List<string> data = new List<string>();
                            MatchCollection collection = regEx.Matches(strings[i] + " ");
                            foreach (Match m in collection)
                            {
                                string mstr = m.Value.Trim(' ', ',');
                                if (mstr.Length > 0)
                                    data.Add(mstr);
                            }

                            if (token == 'M')
                            {
                                if (path.Points.Count > 0)
                                {
                                    paths.Add(path);
                                    path = new SvgPath();
                                }

                                Vector2 startPos = new Vector2
                                {
                                    X = float.Parse(data[0]),
                                    Y = float.Parse(data[1])
                                };

                                relPos = startPos;
                                path.AddPoint(startPos);
                            }
                            else if (token == 'm')
                            {
                                if (path.Points.Count > 0)
                                {
                                    paths.Add(path);
                                    path = new SvgPath();
                                }

                                relPos.X += float.Parse(data[0]);
                                relPos.Y += float.Parse(data[1]);
                                path.AddPoint(relPos);
                            }
                            else if (token == 'C')
                            {
                                for (int j = 0; j < data.Count; j++)
                                {
                                    Vector2 pnt = new Vector2
                                    {
                                        X = float.Parse(data[j++]),
                                        Y = float.Parse(data[j++])
                                    };

                                    path.AddPoint(pnt);
                                    pnt.X = float.Parse(data[j++]);
                                    pnt.Y = float.Parse(data[j++]);
                                    path.AddPoint(pnt);
                                    relPos.X = float.Parse(data[j++]);
                                    relPos.Y = float.Parse(data[j]);
                                    path.AddPoint(relPos);
                                }
                            }
                            else if (token == 'c')
                            {
                                for (int j = 0; j < data.Count; j++)
                                {
                                    Vector2 ctrlPnt = new Vector2
                                    {
                                        X = relPos.X + float.Parse(data[j++]),
                                        Y = relPos.Y + float.Parse(data[j++])
                                    };

                                    path.AddPoint(ctrlPnt);
                                    ctrlPnt.X = relPos.X + float.Parse(data[j++]);
                                    ctrlPnt.Y = relPos.Y + float.Parse(data[j++]);
                                    path.AddPoint(ctrlPnt);
                                    relPos.X += float.Parse(data[j++]);
                                    relPos.Y += float.Parse(data[j]);
                                    path.AddPoint(relPos);
                                }
                            }
                            else if (token == 'L')
                            {
                                for (int j = 0; j < data.Count; j++)
                                {
                                    relPos.X = float.Parse(data[j++]);
                                    relPos.Y = float.Parse(data[j]);
                                    path.AddPoint(relPos);
                                    path.AddPoint(relPos);
                                    path.AddPoint(relPos);
                                }
                            }
                            else if (token == 'l')
                            {
                                for (int j = 0; j < data.Count; j++)
                                {
                                    relPos.X += float.Parse(data[j++]);
                                    relPos.Y += float.Parse(data[j]);
                                    path.AddPoint(relPos);
                                    path.AddPoint(relPos);
                                    path.AddPoint(relPos);
                                }
                            }
                            else if (token == 'V')
                            {
                                for (int j = 0; j < data.Count; j++)
                                {
                                    relPos.Y = float.Parse(data[j]);
                                    path.AddPoint(relPos);
                                    path.AddPoint(relPos);
                                    path.AddPoint(relPos);
                                }
                            }
                            else if (token == 'v')
                            {
                                for (int j = 0; j < data.Count; j++)
                                {
                                    relPos.Y += float.Parse(data[j]);
                                    path.AddPoint(relPos);
                                    path.AddPoint(relPos);
                                    path.AddPoint(relPos);
                                }
                            }
                            else if (token == 'H')
                            {
                                for (int j = 0; j < data.Count; j++)
                                {
                                    relPos.X = float.Parse(data[j]);
                                    path.AddPoint(relPos);
                                    path.AddPoint(relPos);
                                    path.AddPoint(relPos);
                                }
                            }
                            else if (token == 'h')
                            {
                                for (int j = 0; j < data.Count; j++)
                                {
                                    relPos.X += float.Parse(data[j]);
                                    path.AddPoint(relPos);
                                    path.AddPoint(relPos);
                                    path.AddPoint(relPos);
                                }
                            }
                            else if (token == 'S')
                            {
                                for (int j = 0; j < data.Count; j++)
                                {
                                    Vector2 tmpPnt = new Vector2
                                    {
                                        X = relPos.X,
                                        Y = relPos.Y
                                    };

                                    Vector2 ctrlPnt = new Vector2
                                    {
                                        X = float.Parse(data[j++]),
                                        Y = float.Parse(data[j++])
                                    };

                                    relPos.X = float.Parse(data[j++]);
                                    relPos.Y = float.Parse(data[j]);

                                    if (path.Points.Count >= 2)
                                    {
                                        tmpPnt.X = (tmpPnt.X - path.Points[path.Points.Count - 2].X) + tmpPnt.X;
                                        tmpPnt.Y = (tmpPnt.Y - path.Points[path.Points.Count - 2].Y) + tmpPnt.Y;
                                    }

                                    path.AddPoint(tmpPnt);
                                    path.AddPoint(ctrlPnt);
                                    path.AddPoint(relPos);
                                }
                            }
                            else if (token == 's')
                            {
                                for (int j = 0; j < data.Count; j++)
                                {
                                    Vector2 tmpPnt = new Vector2
                                    {
                                        X = relPos.X,
                                        Y = relPos.Y
                                    };

                                    Vector2 ctrlPnt = new Vector2
                                    {
                                        X = relPos.X + float.Parse(data[j++]),
                                        Y = relPos.Y + float.Parse(data[j++])
                                    };

                                    relPos.X += float.Parse(data[j++]);
                                    relPos.Y += float.Parse(data[j]);

                                    if (path.Points.Count >= 2)
                                    {
                                        tmpPnt.X = (tmpPnt.X - path.Points[path.Points.Count - 2].X) + tmpPnt.X;
                                        tmpPnt.Y = (tmpPnt.Y - path.Points[path.Points.Count - 2].Y) + tmpPnt.Y;
                                    }

                                    path.AddPoint(tmpPnt);
                                    path.AddPoint(ctrlPnt);
                                    path.AddPoint(relPos);
                                }
                            }
                            else if (token == 'Z' || token == 'z')
                            {
                                path.Closed = true;
                                relPos.X = path.Points[0].X;
                                relPos.Y = path.Points[0].Y;
                            }
                            else
                            {
                                if (token == 'A' || token == 'a')
                                {
                                    logger.LogWarning("The ArcTo (A/a) command is unsupported at this time.");
                                    warningCount++;
                                }
                                else
                                {
                                    logger.LogWarning("Unsupported token (" + token + ") found.");
                                    warningCount++;
                                }
                            }
                        }
                        paths.Add(path);

                        SvgShape shape = new SvgShape
                        {
                            Fill = fillColor != 0,
                            FillColor = fillColor,
                            Stroke = strokeColor != 0,
                            StrokeColor = strokeColor,
                            Thickness = thickness
                        };

                        shape.AddPaths(paths);
                        image.AddShape(shape);
                    }
                    else
                    {
                        logger.LogWarning("Unsupported element (" + subElem.Name + ") found.");
                        warningCount++;
                    }
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save SVG", "*.svg (SVG File)|*.svg", "Svg", AssetEntry.Filename);
            if (sfd.ShowDialog())
            {
                FrostyTaskWindow.Show("Exporting SVG", "", (task) => 
                {
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
                            foreach (SvgPath path in shape.Paths)
                            {
                                sb.Append("M " + path.Points[0].X.ToString("F3") + "," + path.Points[0].Y.ToString("F3") + " ");
                                sb.Append("C ");
                                for (int i = 1; i < path.Points.Count; i++)
                                    sb.Append(path.Points[i].X.ToString("F3") + "," + path.Points[i].Y.ToString("F3") + " ");

                                if (path.Closed)
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

                        xmlDoc.Save(new FileStream(sfd.FileName, FileMode.Create));
                    }
                });

                logger.Log("Exported {0} to {1}", AssetEntry.Name, sfd.FileName);
            }
        }

        private void FrostySvgImageEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstTimeLoad)
            {
                ulong resRid = ((dynamic)RootObject).Resource;
                image = App.AssetManager.GetResAs<SvgImage>(App.AssetManager.GetResEntry(resRid));
                UpdateCanvas();

                firstTimeLoad = false;
            }
        }

        private void UpdateCanvas()
        {
            Grid imageGrid = new Grid
            {
                Width = image.Width,
                Height = image.Height
            };

            foreach (SvgShape shape in image.Shapes)
            {
                if (shape.Visible == 0)
                    continue;

                Path p = new Path();
                PathGeometry geom = new PathGeometry();
                p.Data = geom;
                if (shape.Stroke)
                    p.Stroke = new SolidColorBrush(Color.FromArgb(
                        (byte)((float)(shape.StrokeColor >> 24) * shape.Opacity),
                        (byte)(shape.StrokeColor),
                        (byte)(shape.StrokeColor >> 8),
                        (byte)(shape.StrokeColor >> 16)));
                if (shape.Fill)
                    p.Fill = new SolidColorBrush(Color.FromArgb(
                        (byte)((float)(shape.FillColor >> 24) * shape.Opacity),
                        (byte)(shape.FillColor),
                        (byte)(shape.FillColor >> 8),
                        (byte)(shape.FillColor >> 16)));
                p.StrokeThickness = shape.Thickness;
                p.StrokeEndLineCap = PenLineCap.Flat;

                foreach (SvgPath path in shape.Paths)
                {
                    PathFigure figure = new PathFigure {StartPoint = new Point(path.Points[0].X, path.Points[0].Y)};

                    PolyBezierSegment segment = new PolyBezierSegment();
                    figure.Segments.Add(segment);

                    for (int i = 1; i < path.Points.Count; i++)
                    {
                        segment.Points.Add(new Point(path.Points[i].X, path.Points[i].Y));
                    }

                    figure.IsClosed = path.Closed;
                    geom.Figures.Add(figure);
                }

                imageGrid.Children.Add(p);
            }

            imageGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            imageGrid.VerticalAlignment = VerticalAlignment.Stretch;

            canvas.Child = imageGrid;
        }

        private uint ConvertToColor(string colorString)
        {
            uint outColor = 0;
            if (colorString.StartsWith("#"))
            {
                colorString = colorString.Remove(0, 1);

                int r = 0x00;
                int g = 0x00;
                int b = 0x00;
                int a = 0xFF;

                if (colorString.Length <= 4)
                {
                    r = byte.Parse("0" + colorString.Substring(0, 1), System.Globalization.NumberStyles.HexNumber) * 17;
                    g = byte.Parse("0" + colorString.Substring(1, 1), System.Globalization.NumberStyles.HexNumber) * 17;
                    b = byte.Parse("0" + colorString.Substring(2, 1), System.Globalization.NumberStyles.HexNumber) * 17;
                    if (colorString.Length == 4)
                        a = byte.Parse("0" + colorString.Substring(3, 1), System.Globalization.NumberStyles.HexNumber) * 17;
                }
                else
                {
                    r = byte.Parse(colorString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    g = byte.Parse(colorString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    b = byte.Parse(colorString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                    if (colorString.Length == 8)
                        a = byte.Parse(colorString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                }

                outColor = (uint)((a << 24) | (b << 16) | (g << 8) | r);
            }
            return outColor;
        }
    }
}
